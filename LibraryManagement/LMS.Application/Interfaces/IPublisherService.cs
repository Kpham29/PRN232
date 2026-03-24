using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LMS.Application.DTOs;

namespace LMS.Application.Interfaces
{
    public interface IPublisherService
    {
        IQueryable<PublisherDto> GetAll();
        Task<PublisherDto> CreateAsync(PublisherDto dto);
        Task UpdateAsync(int id, PublisherDto dto);
        Task DeleteAsync(int id);
    }
}
