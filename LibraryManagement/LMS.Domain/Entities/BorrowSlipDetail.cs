namespace LMS.Domain.Entities;

public class BorrowSlipDetail
{
    public int Id { get; set; }
    public int BorrowSlipId { get; set; }
    public int BookId { get; set; }
    public int Quantity { get; set; } = 1;
    public string Condition { get; set; } = "Good"; // "Good" | "Damaged" | "Lost"

    public BorrowSlip BorrowSlip { get; set; } = null!;
    public Book Book { get; set; } = null!;
}
