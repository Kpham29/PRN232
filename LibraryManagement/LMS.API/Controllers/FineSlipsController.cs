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
public class FineSlipsController : ODataController
{
    private readonly IFineSlipService _service;
    public FineSlipsController(IFineSlipService service) => _service = service;

    [HttpGet, EnableQuery, Authorize(Roles = "Admin,Librarian")]
    public IQueryable<FineSlipDto> Get() => _service.GetAll();

    [HttpGet("mine"), Authorize(Roles = "Reader")]
    public async Task<IActionResult> GetMine()
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        return Ok(await _service.GetByReaderAsync(userId));
    }

    [HttpPost("{id}/pay"), Authorize(Roles = "Admin,Librarian")]
    public async Task<IActionResult> Pay(int id)
    {
        await _service.MarkPaidAsync(id);
        return NoContent();
    }
}
