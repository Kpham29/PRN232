namespace LMS.Application.DTOs;

public class StatisticsDto
{
    public int TotalBooks { get; set; }
    public int TotalReaders { get; set; }
    public int ActiveBorrows { get; set; }
    public int OverdueBorrows { get; set; }
    public int UnpaidFines { get; set; }
    public decimal TotalFineAmount { get; set; }
}
