using LMS.Application.DTOs;

namespace LMS.Application.Interfaces;

public interface IReportService
{
    Task<StatisticsDto> GetStatisticsAsync();
    Task<List<BorrowSlipDto>> GetOverdueAsync();
    Task<List<TopBookDto>> GetTopBooksAsync(int top = 10);
}
