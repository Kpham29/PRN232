using LMS.Application.DTOs;

namespace LMS.Application.Interfaces;

public interface IFineSlipService
{
    IQueryable<FineSlipDto> GetAll();
    Task<List<FineSlipDto>> GetByReaderAsync(int accountId);
    Task AdjustAsync(int id, AdjustFineDto dto);
    Task MarkPaidAsync(int id);
    Task<FineSlipDto> CreateAsync(CreateFineSlipDto dto);
}
