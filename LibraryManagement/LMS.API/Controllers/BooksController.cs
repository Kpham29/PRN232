using LMS.Application.DTOs;
using LMS.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;

namespace LMS.API.Controllers;

[ApiController]
[Route("api/Books")]
public class BooksController : ODataController
{
    private readonly IBookService _books;

    public BooksController(IBookService books) => _books = books;

    [HttpGet, EnableQuery, AllowAnonymous]
    public IQueryable<BookDto> Get() => _books.GetAll();

    [HttpGet("{id}"), AllowAnonymous]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _books.GetByIdAsync(id);
        return result is null ? NotFound() : Ok(result);
    }

    [HttpPost, Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create(CreateBookDto dto)
    {
        var result = await _books.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpPut("{id}"), Authorize(Roles = "Admin")]
    public async Task<IActionResult> Update(int id, CreateBookDto dto)
    {
        await _books.UpdateAsync(id, dto);
        return NoContent();
    }

    [HttpDelete("{id}"), Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        await _books.DeleteAsync(id);
        return NoContent();
    }
}
