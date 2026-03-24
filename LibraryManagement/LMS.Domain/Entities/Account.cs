namespace LMS.Domain.Entities;

public class Account
{
    public int Id { get; set; }
    public string FullName { get; set; } = "";
    public string Email { get; set; } = ""; // Unique
    public string PasswordHash { get; set; } = ""; // BCrypt
    public string Role { get; set; } = ""; // "Admin" | "Librarian" | "Reader"
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
