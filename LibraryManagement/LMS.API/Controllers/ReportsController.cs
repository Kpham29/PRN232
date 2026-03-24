using LMS.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LMS.API.Controllers;

[ApiController]
[Route("api/Reports")]
[Authorize(Roles = "Admin")]
public class ReportsController : ControllerBase
{
    private readonly IReportService _reports;
    public ReportsController(IReportService reports) => _reports = reports;

    [HttpGet("statistics")]
    public async Task<IActionResult> GetStatistics() => Ok(await _reports.GetStatisticsAsync());

    [HttpGet("overdue")]
    public async Task<IActionResult> GetOverdue() => Ok(await _reports.GetOverdueAsync());

    [HttpGet("top-books")]
    public async Task<IActionResult> GetTopBooks() => Ok(await _reports.GetTopBooksAsync());
}
