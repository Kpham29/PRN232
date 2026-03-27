using LMS.Application.DTOs;
using LMS.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;

namespace LMS.API.Controllers;

[ApiController]
[Route("api/Accounts")]
public class AccountsController : ControllerBase
{
    private readonly IAccountService _accounts;
    public AccountsController(IAccountService accounts) => _accounts = accounts;

    [HttpGet, EnableQuery, Authorize(Roles = "Admin")]
    public IQueryable<AccountDto> Get() => _accounts.GetAll();

    [HttpGet("{id}"), Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetById(int id)
    {
        var r = await _accounts.GetByIdAsync(id);
        return r == null ? NotFound() : Ok(r);
    }

    [HttpPost("Librarian"), Authorize(Roles = "Admin")]
    public async Task<IActionResult> CreateLibrarian(CreateLibrarianDto dto)
    {
        var result = await _accounts.CreateLibrarianAsync(dto);
        return Ok(result);
    }

    [HttpPut("{id}/toggle"), Authorize(Roles = "Admin")]
    public async Task<IActionResult> ToggleActive(int id)
    {
        await _accounts.ToggleActiveAsync(id);
        return NoContent();
    }
}
