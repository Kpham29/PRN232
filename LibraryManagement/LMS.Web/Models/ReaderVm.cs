namespace LMS.Web.Models;

public class ReaderVm {
    public int Id { get; set; }
    public string CardNumber { get; set; } = "";
    public string FullName { get; set; } = "";
    public string Gender { get; set; } = "";
    public string? Phone { get; set; }
    public string? Address { get; set; }
    public DateTime CardIssuedDate { get; set; }
    public DateTime CardExpiredDate { get; set; }
    public bool IsCardValid => CardExpiredDate > DateTime.UtcNow;
}
