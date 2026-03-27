namespace LMS.Application.DTOs;

public class ReaderDto
{
    public int Id { get; set; }
    public string CardNumber { get; set; } = "";
    public string FullName { get; set; } = "";
    public DateTime DateOfBirth { get; set; }
    public string Gender { get; set; } = "";
    public string? Occupation { get; set; }
    public string? Address { get; set; }
    public string? Phone { get; set; }
    public DateTime CardIssuedDate { get; set; }
    public DateTime CardExpiredDate { get; set; }
    public bool IsCardActive { get; set; }
    public bool IsActive { get; set; }
}
