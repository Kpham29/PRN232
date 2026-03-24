using LMS.Domain.Entities;
using LMS.Domain.Interfaces;
using LMS.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LMS.Infrastructure.Repositories;

public class BorrowRequestRepository : GenericRepository<BorrowRequest>, IBorrowRequestRepository
{
    public BorrowRequestRepository(AppDbContext ctx) : base(ctx) { }

    public async Task<List<BorrowRequest>> GetByReaderIdAsync(int readerId)
        => await _ctx.BorrowRequests
                     .Include(r => r.Book)
                     .Where(r => r.ReaderId == readerId)
                     .OrderByDescending(r => r.RequestedAt)
                     .ToListAsync();

    public async Task<List<BorrowRequest>> GetPendingAsync()
        => await _ctx.BorrowRequests
                     .Include(r => r.Reader)
                     .Include(r => r.Book)
                     .Where(r => r.Status == "Pending")
                     .OrderBy(r => r.RequestedAt)
                     .ToListAsync();

    public async Task<bool> AnyPendingAsync(int readerId, int bookId)
        => await _ctx.BorrowRequests
                     .AnyAsync(r => r.ReaderId == readerId && r.BookId == bookId && r.Status == "Pending");
}
