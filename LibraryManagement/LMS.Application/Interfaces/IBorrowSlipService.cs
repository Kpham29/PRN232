using LMS.Application.DTOs;

namespace LMS.Application.Interfaces;

public interface IBorrowSlipService
{
    Task<BorrowSlipDto> CreateAsync(CreateBorrowSlipDto dto, int librarianId);
    Task<BorrowSlipDto> GetDtoByIdAsync(int slipId);
    Task ReturnAsync(int slipId);
    Task RenewAsync(int slipId);
    IQueryable<BorrowSlipDto> GetAll();
    Task<List<BorrowSlipDto>> GetByReaderAsync(int accountId);
    Task SynchronizeStatusesAsync();
}
