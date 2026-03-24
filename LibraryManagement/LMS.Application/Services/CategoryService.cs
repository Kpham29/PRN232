using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LMS.Application.DTOs;
using LMS.Application.Interfaces;
using LMS.Domain.Entities;
using LMS.Domain.Interfaces;
using LMS.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace LMS.Application.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _categories;
        private readonly AppDbContext _ctx;

        public CategoryService(ICategoryRepository categories, AppDbContext ctx)
        {
            _categories = categories;
            _ctx = ctx;
        }

        public IQueryable<CategoryDto> GetAll() =>
            _categories.GetAll().Select(c => new CategoryDto { Id = c.Id, Name = c.Name });

        public async Task<CategoryDto> CreateAsync(string name)
        {
            var category = new Category { Name = name };
            await _categories.AddAsync(category);
            await _ctx.SaveChangesAsync();
            return new CategoryDto { Id = category.Id, Name = category.Name };
        }

        public async Task UpdateAsync(int id, string name)
        {
            var category = await _categories.GetByIdAsync(id)
                ?? throw new Exception("Category not found");
            category.Name = name;
            await _ctx.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var category = await _categories.GetByIdAsync(id)
                ?? throw new Exception("Category not found");

            var hasBooks = await _ctx.Books.AnyAsync(b => b.CategoryId == id);
            if (hasBooks)
                throw new Exception("Cannot delete a category that still has books");

            _categories.Delete(category);
            await _ctx.SaveChangesAsync();
        }
    }
}
