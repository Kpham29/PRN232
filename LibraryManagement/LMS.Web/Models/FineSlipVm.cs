namespace LMS.Web.Models;

public class FineSlipVm {
    public int Id { get; set; }
    public string SlipCode { get; set; } = "";
    public string ReaderName { get; set; } = "";
    public string Reason { get; set; } = "";
    public string Status { get; set; } = "";
    public decimal FineAmount { get; set; }
    public decimal? AdjustedAmount { get; set; }
    public decimal ActualAmount => AdjustedAmount ?? FineAmount;
    public DateTime? PaidAt { get; set; }
    public string? Note { get; set; }
}
