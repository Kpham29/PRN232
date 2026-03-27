using System.ComponentModel.DataAnnotations;

namespace LMS.Web.Models;

public class LoginVm {
    [Required][EmailAddress] public string Email    { get; set; } = "";
    [Required]               public string Password { get; set; } = "";
    public string? ErrorMessage { get; set; }
}

public class RegisterVm {
    [Required]               public string   FullName        { get; set; } = "";
    [Required][EmailAddress] public string   Email           { get; set; } = "";
    [Required][MinLength(6)] public string   Password        { get; set; } = "";
    [Compare("Password")]    public string   ConfirmPassword { get; set; } = "";
    [Required]               public DateTime DateOfBirth     { get; set; }
    [Required]               public string   Gender          { get; set; } = "Male";
    [Required][RegularExpression(@"^\d{10}$", ErrorMessage = "Số điện thoại phải có đúng 10 chữ số.")] 
    public string? Phone { get; set; }
    public string? Address { get; set; }
    public string? ErrorMessage { get; set; }
}

public class ChangePasswordVm {
    [Required][DataType(DataType.Password)]
    public string OldPassword { get; set; } = "";
    
    [Required][MinLength(6)][DataType(DataType.Password)]
    public string NewPassword { get; set; } = "";
    
    [Required][Compare("NewPassword")][DataType(DataType.Password)]
    public string ConfirmPassword { get; set; } = "";
    
    public string? ErrorMessage { get; set; }
}
