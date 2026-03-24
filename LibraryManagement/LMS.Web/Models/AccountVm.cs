using System.ComponentModel.DataAnnotations;

namespace LMS.Web.Models;

public class AccountVm {
    public int Id { get; set; }
    public string FullName { get; set; } = "";
    public string Email { get; set; } = "";
    public string Role { get; set; } = "";
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class CreateLibrarianVm {
    [Required]               public string FullName { get; set; } = "";
    [Required][EmailAddress] public string Email { get; set; } = "";
    [Required][MinLength(6)] public string Password { get; set; } = "";
}
