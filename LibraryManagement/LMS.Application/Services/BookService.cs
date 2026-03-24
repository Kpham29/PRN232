using LMS.Application.DTOs;
using LMS.Application.Interfaces;
using LMS.Domain.Entities;
using LMS.Domain.Interfaces;
using LMS.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace LMS.Application.Services;

public class BookService : IBookService
{
    private readonly IBookRepository _books;
    private readonly AppDbContext _ctx;

    public BookService(IBookRepository books, AppDbContext ctx)
    {
        _books = books;
        _ctx = ctx;
    }

    public IQueryable<BookDto> GetAll() =>
        _books.GetAll()
              .Include(b => b.Category)
              .Include(b => b.Publisher)
              .Select(b => new BookDto
              {
                  Id                = b.Id,
                  Title             = b.Title,
                  Author            = b.Author,
                  Price             = b.Price,
                  PublishedYear     = b.PublishedYear,
                  ISBN              = b.ISBN,
                  Description       = b.Description,
                  CoverUrl          = b.CoverUrl,
                  TotalQuantity     = b.TotalQuantity,
                  AvailableQuantity = b.AvailableQuantity,
                  CategoryId        = b.CategoryId,
                  CategoryName      = b.Category.Name,
                  PublisherId       = b.PublisherId,
                  PublisherName     = b.Publisher.Name
              });

    public async Task<BookDto?> GetByIdAsync(int id)
    {
        var b = await _ctx.Books
            .Include(x => x.Category)
            .Include(x => x.Publisher)
            .FirstOrDefaultAsync(x => x.Id == id);
        return b is null ? null : new BookDto
        {
            Id                = b.Id,
            Title             = b.Title,
            Author            = b.Author,
            Price             = b.Price,
            PublishedYear     = b.PublishedYear,
            ISBN              = b.ISBN,
            Description       = b.Description,
            CoverUrl          = b.CoverUrl,
            TotalQuantity     = b.TotalQuantity,
            AvailableQuantity = b.AvailableQuantity,
            CategoryId        = b.CategoryId,
            CategoryName      = b.Category.Name,
            PublisherId       = b.PublisherId,
            PublisherName     = b.Publisher.Name
        };
    }

    public async Task<BookDto> CreateAsync(CreateBookDto dto)
    {
        var book = new Book
        {
            Title             = dto.Title,
            Author            = dto.Author,
            Price             = dto.Price,
            PublishedYear     = dto.PublishedYear,
            ISBN              = dto.ISBN,
            Description       = dto.Description,
            CoverUrl          = dto.CoverUrl,
            TotalQuantity     = dto.TotalQuantity,
            AvailableQuantity = dto.TotalQuantity,
            CategoryId        = dto.CategoryId,
            PublisherId       = dto.PublisherId
        };
        await _books.AddAsync(book);
        await _ctx.SaveChangesAsync();
        return (await GetByIdAsync(book.Id))!;
    }

    public async Task UpdateAsync(int id, CreateBookDto dto)
    {
        var book = await _books.GetByIdAsync(id)
            ?? throw new Exception("Book not found");

        var borrowed = book.TotalQuantity - book.AvailableQuantity;
        if (dto.TotalQuantity < borrowed)
            throw new Exception("New quantity is less than the number currently being borrowed");

        book.Title         = dto.Title;
        book.Author        = dto.Author;
        book.Price         = dto.Price;
        book.PublishedYear = dto.PublishedYear;
        book.ISBN          = dto.ISBN;
        book.Description   = dto.Description;
        book.CoverUrl      = dto.CoverUrl;
        book.CategoryId    = dto.CategoryId;
        book.PublisherId   = dto.PublisherId;

        var diff = dto.TotalQuantity - book.TotalQuantity;
        book.TotalQuantity     = dto.TotalQuantity;
        book.AvailableQuantity = book.AvailableQuantity + diff;

        await _ctx.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var book = await _books.GetByIdAsync(id)
            ?? throw new Exception("Book not found");
        _books.Delete(book);
        await _ctx.SaveChangesAsync();
    }
}