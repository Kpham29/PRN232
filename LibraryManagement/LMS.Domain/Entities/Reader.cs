namespace LMS.Domain.Entities;

public class Reader
{
    public bool IsCardValid => CardExpiredDate > DateTime.UtcNow;


    public int Id { get; set; }
    public int AccountId { get; set; } // FK -> Account
    public string CardNumber { get; set; } = ""; // Unique - VD: LIB20240001
    public string FullName { get; set; } = "";
    public DateTime DateOfBirth { get; set; }
    public string Gender { get; set; } = ""; // "Male" | "Female"
    public string? Occupation { get; set; }
    public string? Address { get; set; }
    public string? Phone { get; set; }
    public string? IdNumber { get; set; }
    public DateTime CardIssuedDate { get; set; } = DateTime.UtcNow;
    public DateTime CardExpiredDate { get; set; } // CardIssuedDate + 1 năm
    
    public Account Account { get; set; } = null!;
    public ICollection<BorrowSlip> BorrowSlips { get; set; } = new List<BorrowSlip>();
}
