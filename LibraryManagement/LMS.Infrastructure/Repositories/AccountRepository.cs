using LMS.Domain.Entities;
using LMS.Domain.Interfaces;
using LMS.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace LMS.Infrastructure.Repositories;

public class AccountRepository : GenericRepository<Account>, IAccountRepository
{
    public AccountRepository(AppDbContext ctx) : base(ctx) { }
    
    public async Task<Account?> GetByEmailAsync(string email)
        => await _ctx.Accounts.FirstOrDefaultAsync(a => a.Email == email);
}
