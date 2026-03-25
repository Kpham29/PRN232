using System.Security.Claims;
using LMS.Application.DTOs;
using LMS.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Routing.Controllers;

namespace LMS.API.Controllers;

[ApiController]
[Route("api/BorrowRequests")]
public class BorrowRequestsController : ODataController
{
    private readonly IBorrowRequestService _service;
    public BorrowRequestsController(IBorrowRequestService service) => _service = service;

    [HttpGet, Authorize(Roles = "Admin,Librarian")]
    public async Task<IActionResult> Get() => Ok(await _service.GetPendingAsync());

    [HttpGet("mine"), Authorize(Roles = "Reader")]
    public async Task<IActionResult> GetMine()
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        return Ok(await _service.GetMyRequestsAsync(userId));
    }

    [HttpPost, Authorize(Roles = "Reader")]
    public async Task<IActionResult> Post([FromBody] CreateBorrowRequestDto dto)
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var result = await _service.SubmitAsync(userId, dto);
        return StatusCode(201, result);
    }

    [HttpPut("{key}/approve"), Authorize(Roles = "Admin,Librarian")]
    public async Task<IActionResult> Approve(int key, [FromBody] ApprovalRequestDto dto)
    {
        var librarianId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        await _service.ApproveAsync(key, librarianId, dto);
        return NoContent();
    }

    [HttpPut("{key}/reject"), Authorize(Roles = "Admin,Librarian")]
    public async Task<IActionResult> Reject(int key, [FromBody] RejectRequestDto dto)
    {
        await _service.RejectAsync(key, dto);
        return NoContent();
    }

    [HttpPut("{key}/cancel"), Authorize(Roles = "Reader")]
    public async Task<IActionResult> Cancel(int key)
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        await _service.CancelAsync(key, userId);
        return NoContent();
    }
}
