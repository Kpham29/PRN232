using LMS.Application.DTOs;

namespace LMS.Application.Interfaces;

public interface IBorrowSlipService
{
    Task<BorrowSlipDto> CreateAsync(CreateBorrowSlipDto dto, int librarianId);
    Task<BorrowSlipDto> GetDtoByIdAsync(int slipId);
    Task ReturnAsync(int slipId);
    IQueryable<BorrowSlipDto> GetAll();
    Task<List<BorrowSlipDto>> GetByReaderAsync(int accountId);
}
