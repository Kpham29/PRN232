using LMS.Web.Models;
using LMS.Web.Services;
using Microsoft.AspNetCore.Mvc;

namespace LMS.Web.Controllers;

public class AccountController : Controller {
    private readonly ApiService _api;
    public AccountController(ApiService api) => _api = api;

    public async Task<IActionResult> Index(string? role) {
        var url = role != null ? $"Accounts?$filter=Role eq '{role}'" : "Accounts";
        ViewBag.Role = role;
        return View(await _api.GetListAsync<AccountVm>(url));
    }

    public IActionResult CreateLibrarian() => View(new CreateLibrarianVm());

    [HttpPost] public async Task<IActionResult> CreateLibrarian(CreateLibrarianVm vm) {
        if (!ModelState.IsValid) return View(vm);
        var (ok, body) = await _api.PostAsync("Accounts/Librarian",
            new { vm.FullName, vm.Email, vm.Password });
        if (!ok) { ModelState.AddModelError("", "Tạo tài khoản thất bại"); return View(vm); }
        TempData["Success"] = "Tạo tài khoản thủ thư thành công!";
        return RedirectToAction("Index");
    }

    [HttpPost] public async Task<IActionResult> Toggle(int id) {
        await _api.PutAsync($"Accounts/{id}/toggle", new { });
        return RedirectToAction("Index");
    }
}
