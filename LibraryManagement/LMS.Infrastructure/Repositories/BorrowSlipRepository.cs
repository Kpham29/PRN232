using LMS.Domain.Entities;
using LMS.Domain.Interfaces;
using LMS.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace LMS.Infrastructure.Repositories;

public class BorrowSlipRepository : GenericRepository<BorrowSlip>, IBorrowSlipRepository
{
    public BorrowSlipRepository(AppDbContext ctx) : base(ctx) { }
    
    public async Task<List<BorrowSlip>> GetByReaderIdAsync(int readerId)
        => await _ctx.BorrowSlips
                     .Include(b => b.Reader)
                     .Include(b => b.Librarian)
                     .Include(b => b.Details).ThenInclude(d => d.Book)
                     .Include(b => b.FineSlip)
                     .Where(b => b.ReaderId == readerId)
                     .OrderByDescending(b => b.BorrowedAt)
                     .ToListAsync();
                     
    public async Task<List<BorrowSlip>> GetOverdueAsync()
        => await _ctx.BorrowSlips
                     .Include(b => b.Reader)
                     .Include(b => b.Librarian)
                     .Include(b => b.Details).ThenInclude(d => d.Book)
                     .Where(b => b.Status == "Borrowing" && b.DueDate < DateTime.UtcNow)
                     .ToListAsync();
}
