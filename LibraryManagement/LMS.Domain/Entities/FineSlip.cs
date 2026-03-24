namespace LMS.Domain.Entities;

public class FineSlip
{
    public int Id { get; set; }
    public int BorrowSlipId { get; set; } // Unique - 1 BorrowSlip -> 1 FineSlip
    public decimal FineAmount { get; set; } // ngày trễ * FineAmountPerDay
    public decimal? AdjustedAmount { get; set; } // Librarian điều chỉnh
    public string Reason { get; set; } = ""; // "Overdue" | "DamagedBook" | "LostBook"
    public string Status { get; set; } = "Unpaid"; // "Unpaid" | "Paid"
    public DateTime? PaidAt { get; set; }
    public string? Note { get; set; }

    public BorrowSlip BorrowSlip { get; set; } = null!;
}
