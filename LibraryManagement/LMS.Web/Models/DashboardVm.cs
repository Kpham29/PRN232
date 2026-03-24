namespace LMS.Web.Models;

public class StatisticsVm {
    public int TotalBooks { get; set; }
    public int TotalReaders { get; set; }
    public int ActiveBorrows { get; set; }
    public int OverdueBorrows { get; set; }
    public int UnpaidFines { get; set; }
    public decimal TotalFineAmount { get; set; }
}

public class TopBookVm {
    public int BookId { get; set; }
    public string Title { get; set; } = "";
    public string Author { get; set; } = "";
    public int BorrowCount { get; set; }
}

public class DashboardVm {
    public StatisticsVm Stats { get; set; } = new();
    public List<BorrowSlipVm> OverdueBooks { get; set; } = new();
    public List<TopBookVm> TopBooks { get; set; } = new();
}
