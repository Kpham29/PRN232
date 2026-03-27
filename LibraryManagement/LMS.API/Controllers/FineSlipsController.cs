using System.Security.Claims;
using LMS.Application.DTOs;
using LMS.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;

namespace LMS.API.Controllers;

[ApiController]
[Route("api/FineSlips")]
public class FineSlipsController : ControllerBase
{
    private readonly IFineSlipService _service;
    private readonly IBorrowSlipService _borrowService;
    public FineSlipsController(IFineSlipService service, IBorrowSlipService borrowService)
    {
        _service = service;
        _borrowService = borrowService;
    }

    [HttpGet, EnableQuery, Authorize(Roles = "Admin,Librarian")]
    public async Task<IQueryable<FineSlipDto>> Get()
    {
        await _borrowService.SynchronizeStatusesAsync();
        return _service.GetAll();
    }

    [HttpGet("mine"), Authorize(Roles = "Reader")]
    public async Task<IActionResult> GetMine()
    {
        await _borrowService.SynchronizeStatusesAsync();
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        return Ok(await _service.GetByReaderAsync(userId));
    }

    [HttpPost, Authorize(Roles = "Admin,Librarian")]
    public async Task<IActionResult> Create(CreateFineSlipDto dto)
    {
        var result = await _service.CreateAsync(dto);
        return Ok(result);
    }

    [HttpPut("{id}/adjust"), Authorize(Roles = "Admin,Librarian")]
    public async Task<IActionResult> Adjust(int id, AdjustFineDto dto)
    {
        await _service.AdjustAsync(id, dto);
        return NoContent();
    }

    [HttpPost("{id}/pay"), Authorize(Roles = "Admin,Librarian")]
    public async Task<IActionResult> Pay(int id)
    {
        await _service.MarkPaidAsync(id);
        return NoContent();
    }
}
