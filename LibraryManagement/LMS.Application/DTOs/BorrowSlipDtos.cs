namespace LMS.Application.DTOs;

public class BorrowSlipDto
{
    public int Id { get; set; }
    public string SlipCode { get; set; } = "";
    public string ReaderName { get; set; } = "";
    public string CardNumber { get; set; } = "";
    public string LibrarianName { get; set; } = "";
    public DateTime BorrowedAt { get; set; }
    public DateTime DueDate { get; set; }
    public DateTime? ReturnedAt { get; set; }
    public string Status { get; set; } = "";
    public List<BorrowSlipDetailDto> Details { get; set; } = new();
    public bool HasFine { get; set; }
    public decimal? FineAmount { get; set; }

    public BorrowSlipDto() { }
    public BorrowSlipDto(int id, string slipCode, string readerName, string cardNumber, string librarianName, DateTime borrowedAt, DateTime dueDate, DateTime? returnedAt, string status, List<BorrowSlipDetailDto> details, bool hasFine, decimal? fineAmount)
    {
        Id = id;
        SlipCode = slipCode;
        ReaderName = readerName;
        CardNumber = cardNumber;
        LibrarianName = librarianName;
        BorrowedAt = borrowedAt;
        DueDate = dueDate;
        ReturnedAt = returnedAt;
        Status = status;
        Details = details;
        HasFine = hasFine;
        FineAmount = fineAmount;
    }
}

public class BorrowSlipDetailDto
{
    public int BookId { get; set; }
    public string BookTitle { get; set; } = "";
    public int Quantity { get; set; }
    public string Condition { get; set; } = "";

    public BorrowSlipDetailDto() { }
    public BorrowSlipDetailDto(int bookId, string bookTitle, int quantity, string condition)
    {
        BookId = bookId;
        BookTitle = bookTitle;
        Quantity = quantity;
        Condition = condition;
    }
}

public class CreateBorrowSlipDto
{
    public int ReaderId { get; set; }
    public List<CreateDetailDto> Books { get; set; } = new();
    public DateTime? BorrowedAt { get; set; }
    public DateTime? DueDate { get; set; }
}

public class CreateDetailDto
{
    public int BookId { get; set; }
    public int Quantity { get; set; }
}
