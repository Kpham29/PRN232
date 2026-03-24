using LMS.Domain.Entities;
using LMS.Domain.Interfaces;
using LMS.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace LMS.Infrastructure.Repositories;

public class PublisherRepository : GenericRepository<Publisher>, IPublisherRepository
{
    public PublisherRepository(AppDbContext ctx) : base(ctx) { }
}
