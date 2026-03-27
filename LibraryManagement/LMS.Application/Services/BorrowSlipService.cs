using LMS.Application.DTOs;
using LMS.Application.Interfaces;
using LMS.Domain.Entities;
using LMS.Domain.Interfaces;
using LMS.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace LMS.Application.Services;

public class BorrowSlipService : IBorrowSlipService
{
    private readonly IBorrowSlipRepository _borrowSlips;
    private readonly IBorrowSlipDetailRepository _details;
    private readonly IBookRepository _books;
    private readonly IReaderRepository _readers;
    private readonly AppDbContext _ctx;

    public BorrowSlipService(
        IBorrowSlipRepository borrowSlips,
        IBorrowSlipDetailRepository details,
        IBookRepository books,
        IReaderRepository readers,
        AppDbContext ctx)
    {
        _borrowSlips = borrowSlips;
        _details = details;
        _books = books;
        _readers = readers;
        _ctx = ctx;
    }

    private static BorrowSlipDto ToDto(BorrowSlip b) => new BorrowSlipDto
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
    };

    public IQueryable<BorrowSlipDto> GetAll() =>
        _ctx.BorrowSlips
            .Include(b => b.Reader)
            .Include(b => b.Librarian)
            .Include(b => b.Details).ThenInclude(d => d.Book)
            .Include(b => b.FineSlip)
            .Select(b => new BorrowSlipDto
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
            HasFine       = b.FineSlip != null,
            FineAmount    = b.FineSlip != null
                            ? (b.FineSlip.AdjustedAmount != null ? b.FineSlip.AdjustedAmount : b.FineSlip.FineAmount)
                            : null
            });

    public async Task<List<BorrowSlipDto>> GetByReaderAsync(int accountId)
    {
        var reader = await _readers.GetByAccountIdAsync(accountId)
            ?? throw new Exception("Reader profile not found for this account");
        var slips = await _borrowSlips.GetByReaderIdAsync(reader.Id);
        return slips.Select(ToDto).ToList();
    }

    public async Task<BorrowSlipDto> GetDtoByIdAsync(int slipId)
    {
        var slip = await _ctx.BorrowSlips
            .Include(b => b.Reader)
            .Include(b => b.Librarian)
            .Include(b => b.Details).ThenInclude(d => d.Book)
            .Include(b => b.FineSlip)
            .FirstOrDefaultAsync(b => b.Id == slipId);
        return ToDto(slip ?? throw new Exception("Borrow slip not found"));
    }

    public async Task<BorrowSlipDto> CreateAsync(CreateBorrowSlipDto dto, int librarianId)
    {
        var reader = await _readers.GetByIdAsync(dto.ReaderId)
            ?? throw new Exception("Reader not found");

        if (!reader.IsCardActive)
            throw new Exception($"Thẻ thư viện của độc giả {reader.FullName} đang bị khóa. Vui lòng mở khóa thẻ trước khi mượn.");

        if (reader.CardExpiredDate <= DateTime.UtcNow)
            throw new Exception($"Thẻ thư viện của độc giả {reader.FullName} đã hết hạn ({reader.CardExpiredDate:dd/MM/yyyy}). Vui lòng gia hạn trước khi mượn.");

        if (dto.Books == null || dto.Books.Count == 0)
            throw new Exception("Phiếu mượn phải có ít nhất 1 cuốn sách.");

        var borrowedAt = dto.BorrowedAt ?? DateTime.UtcNow;
        var dueDate = dto.DueDate ?? borrowedAt.AddDays(14);

        if (dueDate <= borrowedAt)
            throw new Exception("Ngày trả sách không được nhỏ hơn hoặc bằng ngày mượn sách.");

        if (borrowedAt.Date < DateTime.UtcNow.Date)
            throw new Exception("Ngày mượn sách không được trong quá khứ.");

        // Check for overdue books
        var hasOverdue = await _ctx.BorrowSlips.AnyAsync(b => b.ReaderId == dto.ReaderId && b.Status == "Borrowing" && b.DueDate < DateTime.UtcNow);
        if (hasOverdue)
            throw new Exception("Độc giả đang có sách quá hạn chưa trả. Vui lòng trả sách trước khi mượn mới.");

        // Check for unpaid fines
        var hasUnpaidFines = await _ctx.FineSlips.AnyAsync(f => f.BorrowSlip.ReaderId == dto.ReaderId && f.Status == "Unpaid");
        if (hasUnpaidFines)
            throw new Exception("Độc giả đang có phí phạt chưa thanh toán. Vui lòng thanh toán trước khi mượn mới.");

        // Check books available
        foreach (var item in dto.Books)
        {
            if (item.Quantity <= 0)
                throw new Exception("Số lượng sách mượn phải lớn hơn 0.");

            var book = await _books.GetByIdAsync(item.BookId)
                ?? throw new Exception($"Book {item.BookId} not found");
            if (book.AvailableQuantity < item.Quantity)
                throw new Exception($"Not enough stock for book: {book.Title}");
        }

        var slip = new BorrowSlip
        {
            SlipCode    = $"BS-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString()[..6].ToUpper()}",
            ReaderId    = dto.ReaderId,
            LibrarianId = librarianId,
            BorrowedAt  = borrowedAt,
            DueDate     = dueDate,
            Status      = "Borrowing"
        };
        await _borrowSlips.AddAsync(slip);
        await _ctx.SaveChangesAsync();

        foreach (var item in dto.Books)
        {
            var detail = new BorrowSlipDetail
            {
                BorrowSlipId = slip.Id,
                BookId       = item.BookId,
                Quantity     = item.Quantity,
                Condition    = "Good"
            };
            await _details.AddAsync(detail);

            var book = await _books.GetByIdAsync(item.BookId);
            book!.AvailableQuantity -= item.Quantity;
        }
        await _ctx.SaveChangesAsync();

        return (await GetDtoByIdAsync(slip.Id))!;
    }

    public async Task ReturnAsync(int slipId)
    {
        var slip = await _ctx.BorrowSlips
            .Include(b => b.Details).ThenInclude(d => d.Book)
            .FirstOrDefaultAsync(b => b.Id == slipId)
            ?? throw new Exception("Borrow slip not found");

        if (slip.Status == "Returned")
            throw new Exception("This slip has already been returned");

        slip.ReturnedAt = DateTime.UtcNow;
        var isOverdue   = DateTime.UtcNow > slip.DueDate;
        slip.Status     = "Returned";

        // Restore book quantities
        foreach (var detail in slip.Details)
            detail.Book.AvailableQuantity += detail.Quantity;

        // Create fine slip if overdue
        if (isOverdue)
        {
            // Calculate days late based on calendar dates
            var daysLate = (DateTime.UtcNow.Date - slip.DueDate.Date).Days;
            if (daysLate <= 0) daysLate = 1; // If it's same day but late hour, charge 1 day
            
            var fineAmount = daysLate * 5000m; // 5,000 VND per day
            var fine = new FineSlip
            {
                BorrowSlipId = slip.Id,
                FineAmount   = fineAmount,
                Reason       = $"Overdue by {daysLate} day(s)",
                Status       = "Unpaid"
            };
            await _ctx.FineSlips.AddAsync(fine);
        }

        await _ctx.SaveChangesAsync();
    }

    public async Task RenewAsync(int slipId)
    {
        var slip = await _ctx.BorrowSlips
            .FirstOrDefaultAsync(b => b.Id == slipId)
            ?? throw new Exception("Không tìm thấy phiếu mượn.");

        if (slip.Status != "Borrowing")
            throw new Exception("Chỉ có thể gia hạn sách đang trong trạng thái mượn.");

        // Extend DueDate by 7 days from the current DueDate
        slip.DueDate = slip.DueDate.AddDays(7);
        
        await _ctx.SaveChangesAsync();
    }

    public async Task SynchronizeStatusesAsync()
    {
        var now = DateTime.UtcNow;
        var slipsToUpdate = await _ctx.BorrowSlips
            .Include(b => b.FineSlip)
            .Where(b => (b.Status == "Borrowing" && b.DueDate < now) || (b.Status == "Overdue" && b.ReturnedAt == null))
            .ToListAsync();

        if (!slipsToUpdate.Any()) return;

        foreach (var slip in slipsToUpdate)
        {
            if (slip.Status == "Borrowing")
            {
                slip.Status = "Overdue";
            }

            var daysLate = (now.Date - slip.DueDate.Date).Days;
            if (daysLate <= 0) daysLate = 1;
            var currentFineAmount = daysLate * 5000m;

            if (slip.FineSlip == null)
            {
                var fine = new FineSlip
                {
                    BorrowSlipId = slip.Id,
                    FineAmount = currentFineAmount,
                    Reason = "Overdue",
                    Status = "Unpaid"
                };
                _ctx.FineSlips.Add(fine);
                slip.FineSlip = fine; // Link navigation property explicitly
            }
            else if (slip.FineSlip.Status == "Unpaid" && slip.FineSlip.FineAmount < currentFineAmount)
            {
                slip.FineSlip.FineAmount = currentFineAmount;
                slip.FineSlip.Reason = $"Overdue by {daysLate} day(s)";
            }
        }

        await _ctx.SaveChangesAsync();
    }
}
