using LMS.Application.DTOs;
using LMS.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Microsoft.EntityFrameworkCore;

namespace LMS.API.Controllers;

[ApiController]
[Route("api/Publishers")]
public class PublishersController : ControllerBase
{
    private readonly IPublisherService _service;
    public PublishersController(IPublisherService service) => _service = service;

    [HttpGet, EnableQuery, AllowAnonymous]
    public IQueryable<PublisherDto> Get() => _service.GetAll();

    [HttpGet("{id}"), AllowAnonymous]
    public async Task<IActionResult> Get(int id)
    {
        var publisher = await _service.GetAll().Where(p => p.Id == id).FirstOrDefaultAsync();
        return publisher is not null ? Ok(publisher) : NotFound();
    }

    [HttpPost, Authorize(Roles = "Admin")]
    public async Task<IActionResult> Post([FromBody] PublisherDto dto)
    {
        var result = await _service.CreateAsync(dto);
        return StatusCode(201, result);
    }

    [HttpPut("{id}"), Authorize(Roles = "Admin")]
    public async Task<IActionResult> Put(int id, [FromBody] PublisherDto dto)
    {
        await _service.UpdateAsync(id, dto);
        return NoContent();
    }

    [HttpDelete("{id}"), Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        await _service.DeleteAsync(id);
        return NoContent();
    }
}
