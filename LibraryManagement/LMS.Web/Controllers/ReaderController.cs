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

    [HttpGet]
    public async Task<IActionResult> RenewCard()
    {
        var reader = await _api.GetAsync<ReaderVm>("Readers/Me");
        if (reader == null) return RedirectToAction("Login", "Auth");
        return View(reader);
    }

    [HttpPost]
    public async Task<IActionResult> RenewCard(int? id, int months)
    {
        if (id.HasValue) // Manual renewal by Librarian/Admin
        {
            await _api.PostAsync($"Readers/{id}/renew-card?months={months}", new { });
            TempData["Success"] = $"Gia hạn thẻ thêm {months} tháng thành công.";
            return RedirectToAction("Index");
        }
        else // Payment-based renewal by Reader
        {
            var domain = $"{Request.Scheme}://{Request.Host}";
            var res = await _api.PostAsync<object>($"Payments/create-renewal-link?months={months}&domain={domain}", new { });
            if (res.Ok)
            {
                using var doc = System.Text.Json.JsonDocument.Parse(res.Body);
                var url = doc.RootElement.GetProperty("checkoutUrl").GetString();
                return Redirect(url!);
            }
            TempData["Error"] = "Không thể khởi tạo thanh toán. Vui lòng thử lại.";
            return RedirectToAction("MyProfile");
        }
    }

    [HttpPost]
    public async Task<IActionResult> ToggleCardStatus(int id)
    {
        await _api.PostAsync($"Readers/{id}/toggle-card-status", new { });
        TempData["Success"] = "Đã thay đổi trạng thái thẻ.";
        return RedirectToAction("Index");
    }

    public async Task<IActionResult> MyProfile()
    {
        if (string.IsNullOrEmpty(HttpContext.Session.GetString("jwt")))
            return RedirectToAction("Login", "Auth");

        var reader = await _api.GetAsync<ReaderVm>("Readers/Me");
        if (reader == null)
        {
            TempData["Error"] = "Không tìm thấy thông tin hồ sơ độc giả. Vui lòng thử đăng nhập lại.";
            return RedirectToAction("Index", "Home");
        }
        return View(reader);
    }

    [HttpGet] public async Task<IActionResult> GetJson(int id) {
        var r = await _api.GetAsync<ReaderVm>($"Readers/{id}");
        return r == null ? NotFound() : Json(r);
    }
}
