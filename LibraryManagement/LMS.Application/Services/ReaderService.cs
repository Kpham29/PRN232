using LMS.Application.DTOs;
using LMS.Application.Interfaces;
using LMS.Domain.Interfaces;
using LMS.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace LMS.Application.Services;

public class ReaderService : IReaderService
{
    private readonly IReaderRepository _readers;
    private readonly AppDbContext _ctx;

    public ReaderService(IReaderRepository readers, AppDbContext ctx)
    {
        _readers = readers;
        _ctx = ctx;
    }

    public IQueryable<ReaderDto> GetAll() =>
        _ctx.Readers
            .Include(r => r.Account)
            .Select(r => new ReaderDto
            {
                Id              = r.Id,
                CardNumber      = r.CardNumber,
                FullName        = r.FullName,
                DateOfBirth     = r.DateOfBirth,
                Gender          = r.Gender,
                Occupation      = r.Occupation,
                Address         = r.Address,
                Phone           = r.Phone,
                CardIssuedDate  = r.CardIssuedDate,
                CardExpiredDate = r.CardExpiredDate,
                IsCardActive    = r.IsCardActive,
                IsActive        = r.Account.IsActive
            });

    public async Task<ReaderDto?> GetByIdAsync(int id)
    {
        var r = await _ctx.Readers.Include(x => x.Account)
                          .FirstOrDefaultAsync(x => x.Id == id);
        return r is null ? null : new ReaderDto
        {
            Id              = r.Id,
            CardNumber      = r.CardNumber,
            FullName        = r.FullName,
            DateOfBirth     = r.DateOfBirth,
            Gender          = r.Gender,
            Occupation      = r.Occupation,
            Address         = r.Address,
            Phone           = r.Phone,
            CardIssuedDate  = r.CardIssuedDate,
            CardExpiredDate = r.CardExpiredDate,
            IsCardActive    = r.IsCardActive,
            IsActive        = r.Account.IsActive
        };
    }

    public async Task<ReaderDto?> GetByAccountIdAsync(int accountId)
    {
        var r = await _ctx.Readers.Include(x => x.Account)
                          .FirstOrDefaultAsync(x => x.AccountId == accountId);
        return r is null ? null : new ReaderDto
        {
            Id              = r.Id,
            CardNumber      = r.CardNumber,
            FullName        = r.FullName,
            DateOfBirth     = r.DateOfBirth,
            Gender          = r.Gender,
            Occupation      = r.Occupation,
            Address         = r.Address,
            Phone           = r.Phone,
            CardIssuedDate  = r.CardIssuedDate,
            CardExpiredDate = r.CardExpiredDate,
            IsCardActive    = r.IsCardActive,
            IsActive        = r.Account.IsActive
        };
    }

    public async Task RenewCardAsync(int id, int months)
    {
        if (months <= 0)
            throw new Exception("Số tháng gia hạn phải lớn hơn 0.");

        var reader = await _ctx.Readers.FirstOrDefaultAsync(r => r.Id == id)
            ?? throw new Exception("Reader not found");
        var baseDate = reader.CardExpiredDate > DateTime.UtcNow ? reader.CardExpiredDate : DateTime.UtcNow;
        reader.CardExpiredDate = baseDate.AddMonths(months);
        await _ctx.SaveChangesAsync();
    }

    public async Task ToggleCardStatusAsync(int id)
    {
        var reader = await _ctx.Readers.FirstOrDefaultAsync(r => r.Id == id)
            ?? throw new Exception("Reader not found");
        reader.IsCardActive = !reader.IsCardActive;
        await _ctx.SaveChangesAsync();
    }
}
