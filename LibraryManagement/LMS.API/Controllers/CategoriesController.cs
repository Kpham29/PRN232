using LMS.Application.DTOs;
using LMS.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;

namespace LMS.API.Controllers;

[ApiController]
[Route("api/Categories")]
public class CategoriesController : ODataController
{
    private readonly ICategoryService _service;
    public CategoriesController(ICategoryService service) => _service = service;

    [HttpGet, EnableQuery, AllowAnonymous]
    public IQueryable<CategoryDto> Get() => _service.GetAll();

    [HttpPost, Authorize(Roles = "Admin")]
    public async Task<IActionResult> Post([FromBody] CategoryDto dto)
    {
        var result = await _service.CreateAsync(dto.Name);
        return Created(result);
    }
}
