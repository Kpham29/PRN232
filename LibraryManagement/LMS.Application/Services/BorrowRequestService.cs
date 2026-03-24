using LMS.Application.DTOs;
using LMS.Application.Interfaces;
using LMS.Domain.Entities;
using LMS.Domain.Interfaces;
using LMS.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace LMS.Application.Services;

public class BorrowRequestService : IBorrowRequestService
{
    private readonly IBorrowRequestRepository _requests;
    private readonly IReaderRepository _readers;
    private readonly IBookRepository _books;
    private readonly IBorrowSlipService _borrowSlips;
    private readonly AppDbContext _ctx;

    public BorrowRequestService(
        IBorrowRequestRepository requests,
        IReaderRepository readers,
        IBookRepository books,
        IBorrowSlipService borrowSlips,
        AppDbContext ctx)
    {
        _requests = requests;
        _readers = readers;
        _books = books;
        _borrowSlips = borrowSlips;
        _ctx = ctx;
    }

    private static BorrowRequestDto ToDto(BorrowRequest r) => new()
    {
        Id          = r.Id,
        ReaderId    = r.ReaderId,
        ReaderName  = r.Reader?.FullName ?? "",
        BookId      = r.BookId,
        BookTitle   = r.Book?.Title ?? "",
        RequestedAt = r.RequestedAt,
        Status      = r.Status,
        Note        = r.Note
    };

    public async Task<List<BorrowRequestDto>> GetMyRequestsAsync(int accountId)
    {
        var reader = await _readers.GetByAccountIdAsync(accountId)
            ?? throw new Exception("Reader not found");
        var list = await _requests.GetByReaderIdAsync(reader.Id);
        return list.Select(ToDto).ToList();
    }

    public async Task<List<BorrowRequestDto>> GetPendingAsync()
    {
        var list = await _requests.GetPendingAsync();
        return list.Select(ToDto).ToList();
    }

    public async Task<BorrowRequestDto> SubmitAsync(int accountId, CreateBorrowRequestDto dto)
    {
        var reader = await _readers.GetByAccountIdAsync(accountId)
            ?? throw new Exception("Reader profile required to borrow books");
        
        if (!reader.IsCardValid)
            throw new Exception("Your library card is expired");

        // Use the new repository method for efficiency
        if (await _requests.AnyPendingAsync(reader.Id, dto.BookId))
            throw new Exception("You already have a pending request for this book");

        var book = await _books.GetByIdAsync(dto.BookId)
            ?? throw new Exception("Book not found");
        
        if (book.AvailableQuantity <= 0)
            throw new Exception("Book is currently unavailable");

        var req = new BorrowRequest
        {
            ReaderId = reader.Id,
            BookId   = dto.BookId,
            Note     = dto.Note,
            Status   = "Pending",
            RequestedAt = DateTime.UtcNow
        };

        await _requests.AddAsync(req);
        await _ctx.SaveChangesAsync();
        
        // Map directly and load navigation properties if needed, 
        // but for DTO we already have Reader and Book titles from the variables above.
        return new BorrowRequestDto
        {
            Id          = req.Id,
            ReaderId    = reader.Id,
            ReaderName  = reader.FullName,
            BookId      = book.Id,
            BookTitle   = book.Title,
            RequestedAt = req.RequestedAt,
            Status      = req.Status,
            Note        = req.Note
        };
    }

    public async Task ApproveAsync(int requestId, int librarianId, ApprovalRequestDto dto)
    {
        var req = await _ctx.BorrowRequests
            .Include(r => r.Reader)
            .Include(r => r.Book)
            .FirstOrDefaultAsync(r => r.Id == requestId)
            ?? throw new Exception("Request not found");
        
        if (req.Status != "Pending")
            throw new Exception("Request is already processed");

        // Convert to BorrowSlip
        var slipDto = new CreateBorrowSlipDto
        {
            ReaderId   = req.ReaderId,
            Books      = new List<CreateDetailDto> { new() { BookId = req.BookId, Quantity = 1 } },
            BorrowedAt = dto.BorrowedAt,
            DueDate    = dto.DueDate
        };

        await _borrowSlips.CreateAsync(slipDto, librarianId);

        req.Status = "Approved";
        await _ctx.SaveChangesAsync();
    }

    public async Task RejectAsync(int requestId, RejectRequestDto dto)
    {
        var req = await _requests.GetByIdAsync(requestId)
            ?? throw new Exception("Request not found");
        
        if (req.Status != "Pending")
            throw new Exception("Request is already processed");

        req.Status = "Rejected";
        if (!string.IsNullOrWhiteSpace(dto.Reason))
        {
            req.Note = string.IsNullOrEmpty(req.Note) ? $"[Lý do từ chối: {dto.Reason}]" : $"{req.Note} [Lý do từ chối: {dto.Reason}]";
        }
        await _ctx.SaveChangesAsync();
    }

    public async Task CancelAsync(int requestId, int accountId)
    {
        var reader = await _readers.GetByAccountIdAsync(accountId);
        var req = await _requests.GetByIdAsync(requestId)
            ?? throw new Exception("Request not found");
        
        if (req.ReaderId != reader?.Id)
            throw new Exception("Unauthorized");

        if (req.Status != "Pending")
            throw new Exception("Cannot cancel a processed request");

        req.Status = "Cancelled";
        await _ctx.SaveChangesAsync();
    }
}
