using LMS.Domain.Entities;

namespace LMS.Domain.Interfaces;

public interface IFineSlipRepository : IRepository<FineSlip>
{
    Task<List<FineSlip>> GetByReaderIdAsync(int readerId);
}
