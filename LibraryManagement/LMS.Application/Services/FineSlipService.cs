using LMS.Application.DTOs;
using LMS.Application.Interfaces;
using LMS.Domain.Entities;
using LMS.Domain.Interfaces;
using LMS.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace LMS.Application.Services;

public class FineSlipService : IFineSlipService
{
    private readonly IFineSlipRepository _fineSlips;
    private readonly IBorrowSlipRepository _borrowSlips;
    private readonly IReaderRepository _readers;
    private readonly AppDbContext _ctx;

    public FineSlipService(IFineSlipRepository fineSlips,
        IBorrowSlipRepository borrowSlips,
        IReaderRepository readers,
        AppDbContext ctx)
    {
        _fineSlips = fineSlips;
        _borrowSlips = borrowSlips;
        _readers = readers;
        _ctx = ctx;
    }

    public IQueryable<FineSlipDto> GetAll() =>
        _fineSlips.GetAll()
                  .Include(f => f.BorrowSlip).ThenInclude(b => b.Reader)
                  .Select(f => new FineSlipDto
                  {
                      Id             = f.Id,
                      SlipCode       = f.BorrowSlip.SlipCode,
                      ReaderName     = f.BorrowSlip.Reader.FullName,
                      FineAmount     = f.FineAmount,
                      AdjustedAmount = f.AdjustedAmount,
                      Reason         = f.Reason,
                      Status         = f.Status,
                      PaidAt         = f.PaidAt,
                      Note           = f.Note
                  });

    public async Task<List<FineSlipDto>> GetByReaderAsync(int accountId)
    {
        var reader = await _readers.GetByAccountIdAsync(accountId);
        if (reader == null) return new List<FineSlipDto>();

        var list = await _fineSlips.GetByReaderIdAsync(reader.Id);
        return list.Select(f => new FineSlipDto
        {
            Id             = f.Id,
            SlipCode       = f.BorrowSlip.SlipCode,
            ReaderName     = f.BorrowSlip.Reader.FullName,
            FineAmount     = f.FineAmount,
            AdjustedAmount = f.AdjustedAmount,
            Reason         = f.Reason,
            Status         = f.Status,
            PaidAt         = f.PaidAt,
            Note           = f.Note
        }).ToList();
    }

    public async Task AdjustAsync(int id, AdjustFineDto dto)
    {
        var fine = await _fineSlips.GetByIdAsync(id)
            ?? throw new Exception("Fine slip not found");
        if (fine.Status == "Paid")
            throw new Exception("Cannot adjust a fine that has already been paid");

        fine.AdjustedAmount = dto.NewAmount;
        fine.Note = dto.Note;
        await _ctx.SaveChangesAsync();
    }

    public async Task MarkPaidAsync(int id)
    {
        var fine = await _fineSlips.GetByIdAsync(id)
            ?? throw new Exception("Fine slip not found");
        if (fine.Status == "Paid")
            throw new Exception("Fine has already been paid");

        fine.Status = "Paid";
        fine.PaidAt = DateTime.UtcNow;
        await _ctx.SaveChangesAsync();
    }

    public async Task<FineSlipDto> CreateAsync(CreateFineSlipDto dto)
    {
        var borrow = await _borrowSlips.GetByIdAsync(dto.BorrowSlipId)
            ?? throw new Exception("Borrow slip not found");

        var fine = new FineSlip
        {
            BorrowSlipId = dto.BorrowSlipId,
            FineAmount = dto.FineAmount,
            Reason = dto.Reason,
            Note = dto.Note,
            Status = "Unpaid"
        };

        _ctx.FineSlips.Add(fine);
        await _ctx.SaveChangesAsync();

        // Reload to get navigation properties for the DTO
        var f = await _fineSlips.GetAll()
            .Include(x => x.BorrowSlip).ThenInclude(b => b.Reader)
            .FirstOrDefaultAsync(x => x.Id == fine.Id);

        return new FineSlipDto
        {
            Id = f!.Id,
            SlipCode = f.BorrowSlip.SlipCode,
            ReaderName = f.BorrowSlip.Reader.FullName,
            FineAmount = f.FineAmount,
            AdjustedAmount = f.AdjustedAmount,
            Reason = f.Reason,
            Status = f.Status,
            PaidAt = f.PaidAt,
            Note = f.Note
        };
    }
}
