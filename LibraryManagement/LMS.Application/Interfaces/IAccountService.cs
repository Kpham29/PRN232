using LMS.Application.DTOs;

namespace LMS.Application.Interfaces;

public interface IAccountService
{
    IQueryable<AccountDto> GetAll();
    Task<AccountDto?> GetByIdAsync(int id);
    Task<AccountDto> CreateLibrarianAsync(CreateLibrarianDto dto);
    Task ToggleActiveAsync(int id);  // Kích ho?t ho?c khóa tài kho?n
}
