using LMS.Application.DTOs;
using LMS.Application.Interfaces;
using LMS.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace LMS.Application.Services;

public class ReportService : IReportService
{
    private readonly AppDbContext _ctx;

    public ReportService(AppDbContext ctx) => _ctx = ctx;

    public async Task<StatisticsDto> GetStatisticsAsync()
    {
        var totalBooks    = await _ctx.Books.CountAsync();
        var totalReaders  = await _ctx.Readers.CountAsync();
        var activeBorrows = await _ctx.BorrowSlips.CountAsync(b => b.Status == "Borrowing");
        var overdue       = await _ctx.BorrowSlips.CountAsync(b => b.Status == "Overdue");
        var unpaidFines   = await _ctx.FineSlips.CountAsync(f => f.Status == "Unpaid");
        var totalFine     = await _ctx.FineSlips
                                      .Where(f => f.Status == "Unpaid")
                                      .SumAsync(f => f.AdjustedAmount ?? f.FineAmount);

        return new StatisticsDto
        {
            TotalBooks      = totalBooks,
            TotalReaders    = totalReaders,
            ActiveBorrows   = activeBorrows,
            OverdueBorrows  = overdue,
            UnpaidFines     = unpaidFines,
            TotalFineAmount = totalFine
        };
    }

    public async Task<List<BorrowSlipDto>> GetOverdueAsync()
    {
        var slips = await _ctx.BorrowSlips
            .Include(b => b.Reader)
            .Include(b => b.Librarian)
            .Include(b => b.Details).ThenInclude(d => d.Book)
            .Include(b => b.FineSlip)
            .Where(b => b.Status == "Overdue")
            .OrderBy(b => b.DueDate)
            .ToListAsync();

        return slips.Select(b => new BorrowSlipDto
        {
            Id            = b.Id,
            SlipCode      = b.SlipCode,
            ReaderName    = b.Reader.FullName,
            CardNumber    = b.Reader.CardNumber,
            LibrarianName = b.Librarian.FullName,
            BorrowedAt    = b.BorrowedAt,
            DueDate       = b.DueDate,
            ReturnedAt    = b.ReturnedAt,
            Status        = b.Status,
            Details       = b.Details.Select(d => new BorrowSlipDetailDto
            {
                BookId    = d.BookId,
                BookTitle = d.Book.Title,
                Quantity  = d.Quantity,
                Condition = d.Condition
            }).ToList(),
            HasFine    = b.FineSlip != null,
            FineAmount = b.FineSlip?.AdjustedAmount ?? b.FineSlip?.FineAmount
        }).ToList();
    }

    public async Task<List<TopBookDto>> GetTopBooksAsync(int top = 10) =>
        await _ctx.BorrowSlipDetails
                  .Include(d => d.Book)
                  .GroupBy(d => new { d.BookId, d.Book.Title, d.Book.Author })
                  .Select(g => new TopBookDto
                  {
                      BookId      = g.Key.BookId,
                      Title       = g.Key.Title,
                      Author      = g.Key.Author,
                      BorrowCount = g.Sum(d => d.Quantity)
                  })
                  .OrderByDescending(x => x.BorrowCount)
                  .Take(top)
                  .ToListAsync();
}