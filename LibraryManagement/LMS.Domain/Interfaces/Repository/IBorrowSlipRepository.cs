using LMS.Domain.Entities;

namespace LMS.Domain.Interfaces;

public interface IBorrowSlipRepository : IRepository<BorrowSlip>
{
    Task<List<BorrowSlip>> GetByReaderIdAsync(int readerId);
    Task<List<BorrowSlip>> GetOverdueAsync();
}
