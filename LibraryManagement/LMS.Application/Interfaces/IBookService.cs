using LMS.Application.DTOs;

namespace LMS.Application.Interfaces;

public interface IBookService
{
    IQueryable<BookDto> GetAll();
    Task<BookDto?> GetByIdAsync(int id);
    Task<BookDto> CreateAsync(CreateBookDto dto);
    Task UpdateAsync(int id, CreateBookDto dto);
    Task DeleteAsync(int id);
}
