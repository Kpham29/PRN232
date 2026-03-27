using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LMS.Domain.Entities;

public class CardRenewalRequest
{
    public int Id { get; set; }
    public int ReaderId { get; set; }
    [ForeignKey("ReaderId")]
    public Reader? Reader { get; set; }
    
    public int Months { get; set; }
    public decimal Amount { get; set; }
    
    public DateTime RequestedAt { get; set; } = DateTime.Now;
    public DateTime? ProcessedAt { get; set; }
    
    public string Status { get; set; } = "Pending"; // Pending, Paid, Completed, Rejected
    
    public string? PaymentId { get; set; } // PayOS Order ID
    public string? Notes { get; set; }
}
