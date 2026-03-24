using System.ComponentModel.DataAnnotations;

namespace LMS.Domain.Entities;

public class BorrowRequest
{
    public int Id { get; set; }
    
    public int ReaderId { get; set; }
    public Reader Reader { get; set; } = null!;
    
    public int BookId { get; set; }
    public Book Book { get; set; } = null!;
    
    public DateTime RequestedAt { get; set; } = DateTime.UtcNow;
    
    [MaxLength(20)]
    public string Status { get; set; } = "Pending"; // Pending, Approved, Rejected, Cancelled
    
    public string? Note { get; set; }
}
