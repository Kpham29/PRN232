using System.Security.Claims;
using LMS.Application.DTOs;
using LMS.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;

namespace LMS.API.Controllers;

[ApiController]
[Route("api/BorrowSlips")]
public class BorrowSlipsController : ODataController
{
    private readonly IBorrowSlipService _service;
    public BorrowSlipsController(IBorrowSlipService service) => _service = service;

    [HttpGet, EnableQuery, Authorize(Roles = "Admin,Librarian")]
    public IQueryable<BorrowSlipDto> Get() => _service.GetAll();

    [HttpGet("{id}"), Authorize(Roles = "Admin,Librarian")]
    public async Task<IActionResult> GetById(int id)
    {
        var r = await _service.GetDtoByIdAsync(id);
        return r == null ? NotFound() : Ok(r);
    }

    [HttpGet("mine"), Authorize(Roles = "Reader")]
    public async Task<IActionResult> GetMine()
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        return Ok(await _service.GetByReaderAsync(userId));
    }

    [HttpPost, Authorize(Roles = "Admin,Librarian")]
    public async Task<IActionResult> Post([FromBody] CreateBorrowSlipDto dto)
    {
        var libId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var result = await _service.CreateAsync(dto, libId);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpPut("{id}/return"), Authorize(Roles = "Admin,Librarian")]
    public async Task<IActionResult> Return(int id)
    {
        await _service.ReturnAsync(id);
        return NoContent();
    }
}
