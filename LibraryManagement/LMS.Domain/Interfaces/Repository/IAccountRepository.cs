using LMS.Domain.Entities;

namespace LMS.Domain.Interfaces;

public interface IAccountRepository : IRepository<Account>
{
    Task<Account?> GetByEmailAsync(string email);
}
