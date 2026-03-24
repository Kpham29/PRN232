using LMS.Domain.Entities;
using LMS.Domain.Interfaces;
using LMS.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace LMS.Infrastructure.Repositories;

public class FineSlipRepository : GenericRepository<FineSlip>, IFineSlipRepository
{
    public FineSlipRepository(AppDbContext ctx) : base(ctx) { }
    
    public async Task<List<FineSlip>> GetByReaderIdAsync(int readerId)
        => await _ctx.FineSlips
                     .Include(f => f.BorrowSlip).ThenInclude(b => b.Reader)
                     .Where(f => f.BorrowSlip.ReaderId == readerId)
                     .ToListAsync();
}
