namespace LMS.Application.DTOs;

public class FineSlipDto
{
    public int Id { get; set; }
    public string SlipCode { get; set; } = "";
    public string ReaderName { get; set; } = "";
    public decimal FineAmount { get; set; }
    public decimal? AdjustedAmount { get; set; }
    public string Reason { get; set; } = "";
    public string Status { get; set; } = "";
    public DateTime? PaidAt { get; set; }
    public string? Note { get; set; }
}

public class AdjustFineDto
{
    public decimal NewAmount { get; set; }
    public string? Note { get; set; }
}
