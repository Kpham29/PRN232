using LMS.Domain.Entities;

namespace LMS.Domain.Interfaces;

public interface IReaderRepository : IRepository<Reader>
{
    Task<Reader?> GetByCardNumberAsync(string cardNumber);
    Task<Reader?> GetByAccountIdAsync(int accountId);
}
