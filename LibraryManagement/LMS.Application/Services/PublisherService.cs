using LMS.Application.DTOs;
using LMS.Application.Interfaces;
using LMS.Domain.Entities;
using LMS.Domain.Interfaces;
using LMS.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace LMS.Application.Services;

public class PublisherService : IPublisherService
{
    private readonly IPublisherRepository _publishers;
    private readonly AppDbContext _ctx;

    public PublisherService(IPublisherRepository publishers, AppDbContext ctx)
    {
        _publishers = publishers;
        _ctx = ctx;
    }

    public IQueryable<PublisherDto> GetAll() =>
        _publishers.GetAll().Select(p => new PublisherDto
        {
            Id      = p.Id,
            Name    = p.Name,
            Address = p.Address,
            Phone   = p.Phone
        });

    public async Task<PublisherDto?> GetByIdAsync(int id)
    {
        var p = await _publishers.GetByIdAsync(id);
        return p is null ? null : new PublisherDto
        {
            Id      = p.Id,
            Name    = p.Name,
            Address = p.Address,
            Phone   = p.Phone
        };
    }

    public async Task<PublisherDto> CreateAsync(PublisherDto dto)
    {
        var publisher = new Publisher
        {
            Name    = dto.Name,
            Address = dto.Address,
            Phone   = dto.Phone
        };
        await _publishers.AddAsync(publisher);
        await _ctx.SaveChangesAsync();
        return new PublisherDto
        {
            Id      = publisher.Id,
            Name    = publisher.Name,
            Address = publisher.Address,
            Phone   = publisher.Phone
        };
    }

    public async Task UpdateAsync(int id, PublisherDto dto)
    {
        var publisher = await _publishers.GetByIdAsync(id)
            ?? throw new Exception("Publisher not found");
        publisher.Name    = dto.Name;
        publisher.Address = dto.Address;
        publisher.Phone   = dto.Phone;
        await _ctx.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var publisher = await _publishers.GetByIdAsync(id)
            ?? throw new Exception("Publisher not found");

        var hasBooks = await _ctx.Books.AnyAsync(b => b.PublisherId == id);
        if (hasBooks)
            throw new Exception("Cannot delete a publisher that still has books");

        _publishers.Delete(publisher);
        await _ctx.SaveChangesAsync();
    }
}