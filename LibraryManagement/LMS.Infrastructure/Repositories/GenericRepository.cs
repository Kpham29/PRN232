using LMS.Domain.Entities;
using LMS.Domain.Interfaces;
using LMS.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace LMS.Infrastructure.Repositories;

public class GenericRepository<T> : IRepository<T> where T : class
{
    protected readonly AppDbContext _ctx;
    public GenericRepository(AppDbContext ctx) => _ctx = ctx;

    public IQueryable<T> GetAll() => _ctx.Set<T>().AsQueryable();
    public async Task<T?> GetByIdAsync(int id) => await _ctx.Set<T>().FindAsync(id);
    public async Task AddAsync(T entity) => await _ctx.Set<T>().AddAsync(entity);
    public void Update(T entity) => _ctx.Set<T>().Update(entity);
    public void Delete(T entity) => _ctx.Set<T>().Remove(entity);
}
