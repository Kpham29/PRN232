using LMS.Web.Models;
using LMS.Web.Services;
using Microsoft.AspNetCore.Mvc;

namespace LMS.Web.Controllers;

public class FineSlipController : Controller {
    private readonly ApiService _api;
    public FineSlipController(ApiService api) => _api = api;

    public async Task<IActionResult> Index() =>
        View(await _api.GetListAsync<FineSlipVm>("FineSlips"));

    public async Task<IActionResult> Mine() =>
        View(await _api.GetListAsync<FineSlipVm>("FineSlips/mine"));

    [HttpPost] public async Task<IActionResult> Adjust(int id, decimal newAmount, string? note) {
        await _api.PutAsync($"FineSlips/{id}/adjust", new { NewAmount = newAmount, Note = note });
        TempData["Success"] = "Đã cập nhật phí phạt.";
        return RedirectToAction("Index");
    }

    [HttpPost] public async Task<IActionResult> MarkPaid(int id) {
        await _api.PostAsync($"FineSlips/{id}/pay", new { });
        TempData["Success"] = "Đã xác nhận thanh toán.";
        return RedirectToAction("Index");
    }

    [HttpPost] public async Task<IActionResult> PayOnline(int id) {
        var domain = $"{Request.Scheme}://{Request.Host}";
        var res = await _api.PostAsync<object>($"Payments/create-fine-link?fineSlipId={id}&domain={domain}", new { });
        if (res.Ok)
        {
            using var doc = System.Text.Json.JsonDocument.Parse(res.Body);
            var url = doc.RootElement.GetProperty("checkoutUrl").GetString();
            return Redirect(url!);
        }
        TempData["Error"] = "Không thể khởi tạo thanh toán. Vui lòng thử lại.";
        return RedirectToAction("Mine");
    }

    [HttpGet] public async Task<IActionResult> Create() {
        var borrows = await _api.GetListAsync<BorrowSlipVm>("BorrowSlips?$orderby=BorrowedAt desc&$top=50");
        return View(borrows);
    }

    [HttpPost] public async Task<IActionResult> Create(int borrowSlipId, decimal fineAmount, string reason, string? note) {
        var res = await _api.PostAsync("FineSlips", new { BorrowSlipId = borrowSlipId, FineAmount = fineAmount, Reason = reason, Note = note });
        if (res.Ok) {
            TempData["Success"] = "Đã tạo phiếu phạt mới.";
            return RedirectToAction("Index");
        }
        TempData["Error"] = "Không thể tạo phiếu phạt. Có thể phiếu mượn này đã có phiếu phạt.";
        return RedirectToAction("Create");
    }
}
