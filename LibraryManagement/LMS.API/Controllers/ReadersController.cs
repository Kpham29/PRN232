using System.Security.Claims;
using LMS.Application.DTOs;
using LMS.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;

namespace LMS.API.Controllers;

[ApiController]
[Route("api/Readers")]
public class ReadersController : ControllerBase
{
    private readonly IReaderService _readers;
    private readonly IBorrowSlipService _borrows;
    public ReadersController(IReaderService readers, IBorrowSlipService borrows)
    {
        _readers = readers;
        _borrows = borrows;
    }

    [HttpGet, EnableQuery, Authorize(Roles = "Admin,Librarian")]
    public IQueryable<ReaderDto> Get() => _readers.GetAll();

    [HttpGet("{id}"), Authorize(Roles = "Admin,Librarian")]
    public async Task<IActionResult> GetById(int id)
    {
        var r = await _readers.GetByIdAsync(id);
        return r == null ? NotFound() : Ok(r);
    }

    [HttpGet("Me"), Authorize(Roles = "Reader")]
    public async Task<IActionResult> GetMe()
    {
        await _borrows.SynchronizeStatusesAsync();
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var r = await _readers.GetByAccountIdAsync(userId);
        return r == null ? NotFound() : Ok(r);
    }

    [HttpPost("{id}/renew-card"), Authorize(Roles = "Admin,Librarian")]
    public async Task<IActionResult> RenewCard(int id, [FromQuery] int months)
    {
        await _readers.RenewCardAsync(id, months);
        return NoContent();
    }

    [HttpPost("{id}/toggle-card-status"), Authorize(Roles = "Admin,Librarian")]
    public async Task<IActionResult> ToggleCardStatus(int id)
    {
        await _readers.ToggleCardStatusAsync(id);
        return NoContent();
    }

    [HttpGet("GetJson/{id}"), Authorize(Roles = "Admin,Librarian")]
    public async Task<IActionResult> GetJson(int id)
    {
        var r = await _readers.GetByIdAsync(id);
        return r == null ? NotFound() : Ok(r);
    }
}
