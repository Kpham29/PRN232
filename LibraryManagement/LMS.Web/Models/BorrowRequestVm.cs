namespace LMS.Web.Models;

public class BorrowRequestVm
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

public class CreateBorrowRequestVm
{
    public int BookId { get; set; }
    public string? Note { get; set; }
}
