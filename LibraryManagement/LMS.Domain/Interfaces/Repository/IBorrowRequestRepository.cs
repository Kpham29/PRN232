using LMS.Domain.Entities;
using LMS.Domain.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LMS.Domain.Interfaces;

public interface IBorrowRequestRepository : IRepository<BorrowRequest>
{
    Task<List<BorrowRequest>> GetByReaderIdAsync(int readerId);
    Task<List<BorrowRequest>> GetPendingAsync();
    Task<bool> AnyPendingAsync(int readerId, int bookId);
}
