using LMS.Domain.Entities;
using LMS.Domain.Interfaces;
using LMS.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace LMS.Infrastructure.Repositories;

public class ReaderRepository : GenericRepository<Reader>, IReaderRepository
{
    public ReaderRepository(AppDbContext ctx) : base(ctx) { }
    
    public async Task<Reader?> GetByCardNumberAsync(string cardNumber)
        => await _ctx.Readers.Include(r => r.Account)
                             .FirstOrDefaultAsync(r => r.CardNumber == cardNumber);
                             
    public async Task<Reader?> GetByAccountIdAsync(int accountId)
        => await _ctx.Readers.FirstOrDefaultAsync(r => r.AccountId == accountId);
}
