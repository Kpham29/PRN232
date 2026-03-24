using LMS.Application.DTOs;

namespace LMS.Application.Interfaces;

public interface IBorrowRequestService
{
    Task<List<BorrowRequestDto>> GetMyRequestsAsync(int accountId);
    Task<List<BorrowRequestDto>> GetPendingAsync();
    Task<BorrowRequestDto> SubmitAsync(int accountId, CreateBorrowRequestDto dto);
    Task ApproveAsync(int requestId, int librarianId, ApprovalRequestDto dto);
    Task RejectAsync(int requestId, RejectRequestDto dto);
    Task CancelAsync(int requestId, int accountId);
}
