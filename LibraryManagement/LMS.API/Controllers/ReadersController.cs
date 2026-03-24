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
public class ReadersController : ODataController
{
    private readonly IReaderService _readers;
    public ReadersController(IReaderService readers) => _readers = readers;

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
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var r = await _readers.GetByAccountIdAsync(userId);
        return r == null ? NotFound() : Ok(r);
    }

    [HttpPost("Me/Renew"), Authorize(Roles = "Reader")]
    public async Task<IActionResult> RenewMyCard()
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var r = await _readers.GetByAccountIdAsync(userId);
        if (r == null) return NotFound();
        await _readers.RenewCardAsync(r.Id);
        return NoContent();
    }

    [HttpPost("{id}/renew-card"), Authorize(Roles = "Admin,Librarian")]
    public async Task<IActionResult> RenewCard(int id)
    {
        await _readers.RenewCardAsync(id);
        return NoContent();
    }

    [HttpGet("GetJson/{id}"), Authorize(Roles = "Admin,Librarian")]
    public async Task<IActionResult> GetJson(int id)
    {
        var r = await _readers.GetByIdAsync(id);
        return r == null ? NotFound() : Ok(r);
    }
}
