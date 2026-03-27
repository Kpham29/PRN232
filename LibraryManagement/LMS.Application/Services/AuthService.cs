using LMS.Application.DTOs;
using LMS.Domain.Entities;
using LMS.Application.Interfaces;
using LMS.Domain.Interfaces;
using LMS.Infrastructure.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace LMS.Application.Services;

public class AuthService : IAuthService
{
    private readonly IAccountRepository _accounts;
    private readonly IReaderRepository _readers;
    private readonly AppDbContext _ctx;
    private readonly IConfiguration _config;

    public AuthService(IAccountRepository accounts, IReaderRepository readers, AppDbContext ctx, IConfiguration config)
    {
        _accounts = accounts;
        _readers = readers;
        _ctx = ctx;
        _config = config;
    }

    public async Task<AuthResponseDto> RegisterAsync(RegisterDto dto)
    {
        if (await _accounts.GetByEmailAsync(dto.Email) != null)
            throw new Exception("Email already exists");

        if (dto.DateOfBirth > DateTime.UtcNow.AddYears(-6))
            throw new Exception("Người dùng phải từ 6 tuổi trở lên.");

        if (string.IsNullOrEmpty(dto.Phone) || dto.Phone.Length != 10 || !dto.Phone.All(char.IsDigit))
            throw new Exception("Số điện thoại phải có đúng 10 chữ số.");

        var account = new Account
        {
            FullName = dto.FullName,
            Email = dto.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
            Role = "Reader"
        };
        await _accounts.AddAsync(account);
        await _ctx.SaveChangesAsync();

        await _readers.AddAsync(new Reader
        {
            AccountId = account.Id,
            CardNumber = $"LIB{DateTime.Now:yyyyMMdd}{account.Id:D4}",
            FullName = dto.FullName,
            DateOfBirth = dto.DateOfBirth,
            Gender = dto.Gender,
            Phone = dto.Phone,
            Address = dto.Address,
            CardExpiredDate = DateTime.UtcNow.AddYears(1)
        });
        await _ctx.SaveChangesAsync();
        return new AuthResponseDto(GenerateToken(account), account.Role, account.FullName, account.Id);
    }

    public async Task<AuthResponseDto> LoginAsync(LoginDto dto)
    {
        var account = await _accounts.GetByEmailAsync(dto.Email)
            ?? throw new Exception("Email not found");
        if (!account.IsActive) throw new Exception("Account is deactivated");
        if (!BCrypt.Net.BCrypt.Verify(dto.Password, account.PasswordHash))
            throw new Exception("Incorrect password");
        return new AuthResponseDto(GenerateToken(account), account.Role, account.FullName, account.Id);
    }

    public async Task ChangePasswordAsync(int userId, ChangePasswordDto dto)
    {
        var account = await _accounts.GetByIdAsync(userId)
            ?? throw new Exception("Account not found");
        if (!BCrypt.Net.BCrypt.Verify(dto.OldPassword, account.PasswordHash))
            throw new Exception("Incorrect old password");
        account.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.NewPassword);
        _accounts.Update(account);
        await _ctx.SaveChangesAsync();
    }

    private string GenerateToken(Account a)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["JwtSettings:SecretKey"]!));
        var token = new JwtSecurityToken(
            issuer: _config["JwtSettings:Issuer"], audience: _config["JwtSettings:Audience"],
            claims: new[] {
                new Claim(ClaimTypes.NameIdentifier, a.Id.ToString()),
                new Claim(ClaimTypes.Email,          a.Email),
                new Claim(ClaimTypes.Role,           a.Role),
                new Claim(ClaimTypes.Name,           a.FullName)
            },
            expires: DateTime.UtcNow.AddMinutes(int.Parse(_config["JwtSettings:ExpiryInMinutes"]!)),
            signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256));
        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
