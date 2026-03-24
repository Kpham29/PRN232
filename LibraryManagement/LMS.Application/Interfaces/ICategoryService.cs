using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LMS.Application.DTOs;

namespace LMS.Application.Interfaces;

public interface ICategoryService
{
    IQueryable<CategoryDto> GetAll();
    Task<CategoryDto> CreateAsync(string name);
    Task UpdateAsync(int id, string name);
    Task DeleteAsync(int id);
}
