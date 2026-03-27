using LMS.Application.DTOs;
using LMS.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Microsoft.EntityFrameworkCore;

namespace LMS.API.Controllers;

[ApiController]
[Route("api/Categories")]
public class CategoriesController : ControllerBase
{
    private readonly ICategoryService _service;
    public CategoriesController(ICategoryService service) => _service = service;

    [HttpGet, EnableQuery, AllowAnonymous]
    public IQueryable<CategoryDto> Get() => _service.GetAll();

    [HttpGet("{id}"), AllowAnonymous]
    public async Task<IActionResult> Get(int id)
    {
        var category = await _service.GetAll().Where(c => c.Id == id).FirstOrDefaultAsync();
        return category is not null ? Ok(category) : NotFound();
    }

    [HttpPost, Authorize(Roles = "Admin")]
    public async Task<IActionResult> Post([FromBody] CategoryDto dto)
    {
        var result = await _service.CreateAsync(dto.Name);
        return StatusCode(201, result);
    }

    [HttpPut("{id}"), Authorize(Roles = "Admin")]
    public async Task<IActionResult> Put(int id, [FromBody] CategoryDto dto)
    {
        await _service.UpdateAsync(id, dto.Name);
        return NoContent();
    }

    [HttpDelete("{id}"), Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        await _service.DeleteAsync(id);
        return NoContent();
    }
}
