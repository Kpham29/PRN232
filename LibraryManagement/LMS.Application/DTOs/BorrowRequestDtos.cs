namespace LMS.Application.DTOs;

public class BorrowRequestDto
{
    public int Id { get; set; }
    public int ReaderId { get; set; }
    public string ReaderName { get; set; } = "";
    public int BookId { get; set; }
    public string BookTitle { get; set; } = "";
    public DateTime RequestedAt { get; set; }
    public string Status { get; set; } = "";
    public string? Note { get; set; }
}

public class CreateBorrowRequestDto
{
    public int BookId { get; set; }
    public string? Note { get; set; }
}

public class RejectRequestDto
{
    public string Reason { get; set; } = "";
}

public class ApprovalRequestDto
{
    public DateTime? BorrowedAt { get; set; }
    public DateTime? DueDate { get; set; }
}
