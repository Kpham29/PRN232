namespace LMS.Application.DTOs;

public class LoginDto
{
    public string Email { get; set; } = "";
    public string Password { get; set; } = "";
}

public class RegisterDto
{
    public string FullName { get; set; } = "";
    public string Email { get; set; } = "";
    public string Password { get; set; } = "";
    public DateTime DateOfBirth { get; set; }
    public string Gender { get; set; } = "";
    public string? Phone { get; set; }
    public string? Address { get; set; }
}

public class AuthResponseDto
{
    public string Token { get; set; } = "";
    public string Role { get; set; } = "";
    public string FullName { get; set; } = "";
    public int UserId { get; set; }

    public AuthResponseDto() { }
    public AuthResponseDto(string token, string role, string fullName, int userId)
    {
        Token = token;
        Role = role;
        FullName = fullName;
        UserId = userId;
    }
}

public class ChangePasswordDto
{
    public string OldPassword { get; set; } = "";
    public string NewPassword { get; set; } = "";
}
