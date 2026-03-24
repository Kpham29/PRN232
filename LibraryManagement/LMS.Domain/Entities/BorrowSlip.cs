namespace LMS.Domain.Entities;

public class BorrowSlip
{
    public int Id { get; set; }
    public string SlipCode { get; set; } = ""; // Unique - VD: BS-2024-0001
    public int ReaderId { get; set; }
    public int LibrarianId { get; set; } // FK -> Account
    public DateTime BorrowedAt { get; set; } = DateTime.UtcNow;
    public DateTime DueDate { get; set; } // BorrowedAt + 14 ngày
    public DateTime? ReturnedAt { get; set; } // null = chưa trả
    public string Status { get; set; } = "Borrowing"; // "Borrowing" | "Returned" | "Overdue"
    public string? Note { get; set; }

    public Reader Reader { get; set; } = null!;
    public Account Librarian { get; set; } = null!;
    public ICollection<BorrowSlipDetail> Details { get; set; } = new List<BorrowSlipDetail>();
    public FineSlip? FineSlip { get; set; }
}
