using LMS.Web.Models;
using LMS.Web.Services;
using Microsoft.AspNetCore.Mvc;

namespace LMS.Web.Controllers;

public class BorrowSlipController : Controller {
    private readonly ApiService _api;
    public BorrowSlipController(ApiService api) => _api = api;

    public async Task<IActionResult> Index(string? status) {
        var url = status != null ? $"BorrowSlips?$filter=Status eq '{status}'" : "BorrowSlips";
        ViewBag.Status = status;
        return View(await _api.GetListAsync<BorrowSlipVm>(url));
    }

    public async Task<IActionResult> Mine() =>
        View(await _api.GetListAsync<BorrowSlipVm>("BorrowSlips/mine"));

    public async Task<IActionResult> Create() => View(new CreateBorrowSlipVm {
        ReaderList = await _api.GetListAsync<ReaderVm>("Readers"),
        BookList   = await _api.GetListAsync<BookVm>("Books?$filter=AvailableQuantity gt 0")
    });

    [HttpPost] public async Task<IActionResult> Create(CreateBorrowSlipVm vm) {
        var (ok, body) = await _api.PostAsync("BorrowSlips", new { 
            vm.ReaderId, 
            Books = vm.Items,
            vm.BorrowedAt,
            vm.DueDate
        });
        TempData[ok ? "Success" : "Error"] = ok ? "Tạo phiếu mượn thành công!" : "Lỗi: " + body;
        return RedirectToAction("Index");
    }

    [HttpPost] public async Task<IActionResult> Return(int id) {
        var (ok, body) = await _api.PutAsync($"BorrowSlips/{id}/return", new { });
        TempData[ok ? "Success" : "Error"] = ok ? "Xác nhận trả sách thành công!" : "Lỗi: " + body;
        return RedirectToAction("Index");
    }

    [HttpPost] public async Task<IActionResult> Renew(int id) {
        var (ok, body) = await _api.PutAsync($"BorrowSlips/{id}/renew", new { });
        TempData[ok ? "Success" : "Error"] = ok ? "Gia hạn sách thành công! Hạn trả mới đã được cập nhật." : "Lỗi: " + body;
        return RedirectToAction("Mine");
    }

    public async Task<IActionResult> Requests() =>
        View(await _api.GetListAsync<BorrowRequestVm>("BorrowRequests"));

    public async Task<IActionResult> MyRequests() =>
        View(await _api.GetListAsync<BorrowRequestVm>("BorrowRequests/mine"));

    [HttpPost] public async Task<IActionResult> ApproveRequest(int id, DateTime? dueDate) {
        var (ok, body) = await _api.PutAsync($"BorrowRequests/{id}/Approve", new { DueDate = dueDate });
        if (ok) TempData["Success"] = "Đã duyệt yêu cầu!";
        else TempData["Error"] = "Lỗi: " + (string.IsNullOrWhiteSpace(body) ? "Không có phản hồi từ hệ thống (kiểm tra quyền hạn)." : body);
        return RedirectToAction("Requests");
    }


    [HttpPost] public async Task<IActionResult> CancelRequest(int id) {
        var (ok, body) = await _api.PutAsync($"BorrowRequests/{id}/cancel", new { });
        TempData[ok ? "Success" : "Error"] = ok ? "Đã hủy yêu cầu thành công." : "Lỗi: " + body;
        return RedirectToAction("MyRequests");
    }

    [HttpPost] public async Task<IActionResult> RejectRequest(int id, string reason) {
        var (ok, body) = await _api.PutAsync($"BorrowRequests/{id}/Reject", new { Reason = reason });
        TempData[ok ? "Success" : "Error"] = ok ? "Đã từ chối yêu cầu." : "Lỗi: " + body;
        return RedirectToAction("Requests");
    }
}
