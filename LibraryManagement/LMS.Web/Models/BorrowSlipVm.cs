using System.ComponentModel.DataAnnotations;

namespace LMS.Web.Models;

public class BorrowSlipVm {
    public int Id { get; set; }
    public string SlipCode { get; set; } = "";
    public string ReaderName { get; set; } = "";
    public string CardNumber { get; set; } = "";
    public string LibrarianName { get; set; } = "";
    public string Status { get; set; } = "";
    public DateTime BorrowedAt { get; set; }
    public DateTime DueDate { get; set; }
    public DateTime? ReturnedAt { get; set; }
    public List<BorrowDetailVm> Details { get; set; } = new();
    public bool HasFine { get; set; }
    public decimal? FineAmount { get; set; }
    public bool IsOverdue => Status == "Borrowing" && DueDate < DateTime.Now;
}

public class BorrowDetailVm {
    public string BookTitle { get; set; } = "";
    public int Quantity { get; set; }
}

public class CreateBorrowSlipVm {
    [Required] public int ReaderId { get; set; }
    public List<BorrowItemVm> Items { get; set; } = new();
    public List<ReaderVm> ReaderList { get; set; } = new();
    public List<BookVm> BookList { get; set; } = new();
}

public class BorrowItemVm {
    public int BookId { get; set; }
    public int Quantity { get; set; } = 1;
}
