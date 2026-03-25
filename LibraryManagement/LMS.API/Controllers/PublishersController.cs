using LMS.Application.DTOs;
using LMS.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;

namespace LMS.API.Controllers;

[ApiController]
[Route("api/Publishers")]
public class PublishersController : ODataController
{
    private readonly IPublisherService _service;
    public PublishersController(IPublisherService service) => _service = service;

    [HttpGet, EnableQuery, AllowAnonymous]
    public IQueryable<PublisherDto> Get() => _service.GetAll();

    [HttpPost, Authorize(Roles = "Admin")]
    public async Task<IActionResult> Post([FromBody] PublisherDto dto)
    {
        var result = await _service.CreateAsync(dto);
        return StatusCode(201, result);
    }
}
