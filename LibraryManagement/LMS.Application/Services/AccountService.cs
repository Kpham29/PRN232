using LMS.Application.DTOs;
using LMS.Application.Interfaces;
using LMS.Domain.Entities;
using LMS.Domain.Interfaces;
using LMS.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace LMS.Application.Services;

public class AccountService : IAccountService
{
    private readonly IAccountRepository _accounts;
    private readonly AppDbContext _ctx;

    public AccountService(IAccountRepository accounts, AppDbContext ctx)
    {
        _accounts = accounts;
        _ctx = ctx;
    }

    public IQueryable<AccountDto> GetAll() =>
        _accounts.GetAll().Select(a => new AccountDto
        {
            Id        = a.Id,
            FullName  = a.FullName,
            Email     = a.Email,
            Role      = a.Role,
            IsActive  = a.IsActive,
            CreatedAt = a.CreatedAt
        });

    public async Task<AccountDto?> GetByIdAsync(int id)
    {
        var a = await _accounts.GetByIdAsync(id);
        return a is null ? null : new AccountDto
        {
            Id        = a.Id,
            FullName  = a.FullName,
            Email     = a.Email,
            Role      = a.Role,
            IsActive  = a.IsActive,
            CreatedAt = a.CreatedAt
        };
    }

    public async Task<AccountDto> CreateLibrarianAsync(CreateLibrarianDto dto)
    {
        dto.Email = dto.Email.Trim();
        dto.FullName = dto.FullName.Trim();

        if (await _accounts.GetByEmailAsync(dto.Email) != null)
            throw new Exception("Email already exists");

        var account = new Account
        {
            FullName     = dto.FullName,
            Email        = dto.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
            Role         = "Librarian",
            IsActive     = true,
            CreatedAt    = DateTime.UtcNow
        };
        await _accounts.AddAsync(account);
        await _ctx.SaveChangesAsync();
        return new AccountDto
        {
            Id        = account.Id,
            FullName  = account.FullName,
            Email     = account.Email,
            Role      = account.Role,
            IsActive  = account.IsActive,
            CreatedAt = account.CreatedAt
        };
    }

    public async Task ToggleActiveAsync(int id)
    {
        var account = await _accounts.GetByIdAsync(id)
            ?? throw new Exception("Account not found");
        account.IsActive = !account.IsActive;
        await _ctx.SaveChangesAsync();
    }
}
