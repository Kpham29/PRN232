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
        await _api.PutAsync($"FineSlips/{id}/pay", new { });
        TempData["Success"] = "Đã xác nhận thanh toán.";
        return RedirectToAction("Index");
    }
}
