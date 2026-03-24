using LMS.Domain.Entities;
using LMS.Domain.Interfaces;
using LMS.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace LMS.Infrastructure.Repositories;

public class BorrowSlipDetailRepository : GenericRepository<BorrowSlipDetail>, IBorrowSlipDetailRepository
{
    public BorrowSlipDetailRepository(AppDbContext ctx) : base(ctx) { }
}
