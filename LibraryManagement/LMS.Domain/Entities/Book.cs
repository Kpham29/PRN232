namespace LMS.Domain.Entities;

public class Book
{
    public int Id { get; set; }
    public string Title { get; set; } = "";
    public string Author { get; set; } = "";
    public decimal Price { get; set; }
    public int PublishedYear { get; set; }
    public string? ISBN { get; set; }
    public string? Description { get; set; }
    public string? CoverUrl { get; set; }
    public int TotalQuantity { get; set; } // Tổng số bản
    public int AvailableQuantity { get; set; } // Số bản có thể mượn
    
    public int PublisherId { get; set; }
    public int CategoryId { get; set; }
    public DateTime ImportedAt { get; set; } = DateTime.UtcNow;

    public Publisher Publisher { get; set; } = null!;
    public Category Category { get; set; } = null!;
    public ICollection<BorrowSlipDetail> BorrowSlipDetails { get; set; } = new List<BorrowSlipDetail>();
}
