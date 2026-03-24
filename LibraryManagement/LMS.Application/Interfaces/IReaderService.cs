using LMS.Application.DTOs;

namespace LMS.Application.Interfaces;

public interface IReaderService
{
    IQueryable<ReaderDto> GetAll();
    Task<ReaderDto?> GetByIdAsync(int id);
    Task<ReaderDto?> GetByAccountIdAsync(int accountId);
    Task RenewCardAsync(int id);  // Gia h?n th? thêm 1 n?m tính t? ngày hi?n t?i
}
