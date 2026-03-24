using LMS.Web.Models;
using LMS.Web.Services;
using Microsoft.AspNetCore.Mvc;

namespace LMS.Web.Controllers;

public class ReaderController : Controller {
    private readonly ApiService _api;
    public ReaderController(ApiService api) => _api = api;

    public async Task<IActionResult> Index(string? search) {
        var url = string.IsNullOrWhiteSpace(search) ? "Readers"
            : $"Readers?$filter=contains(FullName,'{search}') or contains(CardNumber,'{search}')";
        ViewBag.Search = search;
        return View(await _api.GetListAsync<ReaderVm>(url));
    }

    public async Task<IActionResult> Detail(int id) =>
        await _api.GetAsync<ReaderVm>($"Readers/{id}") is { } r ? View(r) : NotFound();

    [HttpPost] public async Task<IActionResult> RenewCard(int id) {
        await _api.PostAsync($"Readers/{id}/renew-card", new { });
        TempData["Success"] = "Gia hạn thẻ thêm 1 năm thành công.";
        return RedirectToAction("Detail", new { id });
    }

    public async Task<IActionResult> MyProfile() {
        var reader = await _api.GetAsync<ReaderVm>("Readers/Me");
        if (reader == null) return RedirectToAction("Login", "Auth");
        return View(reader);
    }

    [HttpPost] public async Task<IActionResult> RenewMyCard() {
        var (ok, body) = await _api.PostAsync("Readers/Me/Renew", new { });
        if (!ok) TempData["Error"] = "Lỗi gia hạn: " + body;
        else TempData["Success"] = "Bạn đã tự gia hạn thẻ thành công thêm 1 năm!";
        return RedirectToAction("MyProfile");
    }

    [HttpGet] public async Task<IActionResult> GetJson(int id) {
        var r = await _api.GetAsync<ReaderVm>($"Readers/{id}");
        return r == null ? NotFound() : Json(r);
    }
}
