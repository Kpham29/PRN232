using LMS.Application.DTOs;

namespace LMS.Application.Interfaces;

public interface IFineSlipService
{
    IQueryable<FineSlipDto> GetAll();
    Task<List<FineSlipDto>> GetByReaderAsync(int readerId);
    Task AdjustAsync(int id, AdjustFineDto dto);     // Librarian ?i?u ch?nh s? ti?n
    Task MarkPaidAsync(int id);                       // Librarian xác nh?n ?ã thu ti?n
}
