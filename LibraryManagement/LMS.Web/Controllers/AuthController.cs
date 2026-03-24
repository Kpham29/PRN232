using System.Text.Json;
using LMS.Web.Models;
using LMS.Web.Services;
using Microsoft.AspNetCore.Mvc;

namespace LMS.Web.Controllers;

public class AuthController : Controller {
    private readonly ApiService _api;
    public AuthController(ApiService api) => _api = api;

    [HttpGet] public IActionResult Login()    => View(new LoginVm());
    [HttpGet] public IActionResult Register() => View(new RegisterVm());

    [HttpPost] public async Task<IActionResult> Login(LoginVm vm) {
        if (!ModelState.IsValid) return View(vm);
        var (ok, body) = await _api.PostAsync("Auth/Login", new { vm.Email, vm.Password });
        if (!ok) { vm.ErrorMessage = "Email hoặc mật khẩu không đúng"; return View(vm); }
        var r = JsonSerializer.Deserialize<AuthResponseDto>(body,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true })!;
        HttpContext.Session.SetString("jwt",      r.Token);
        HttpContext.Session.SetString("role",     r.Role);
        HttpContext.Session.SetString("fullName", r.FullName);
        HttpContext.Session.SetInt32 ("userId",   r.UserId);
        return r.Role switch {
            "Admin"     => RedirectToAction("Dashboard", "Home"),
            "Librarian" => RedirectToAction("Index", "BorrowSlip"),
            _           => RedirectToAction("Index", "Book")
        };
    }

    [HttpPost] public async Task<IActionResult> Register(RegisterVm vm) {
        if (!ModelState.IsValid) return View(vm);
        var (ok, _) = await _api.PostAsync("Auth/Register",
            new { vm.FullName, vm.Email, vm.Password, vm.DateOfBirth, vm.Gender, vm.Phone, vm.Address });
        if (!ok) { vm.ErrorMessage = "Đăng ký thất bại. Email có thể đã tồn tại."; return View(vm); }
        TempData["Success"] = "Đăng ký thành công! Vui lòng đăng nhập.";
        return RedirectToAction("Login");
    }

    public IActionResult Logout() { HttpContext.Session.Clear(); return RedirectToAction("Login"); }

    [HttpGet] public IActionResult ChangePassword() {
        if (HttpContext.Session.GetString("jwt") == null) return RedirectToAction("Login");
        return View(new ChangePasswordVm());
    }

    [HttpPost] public async Task<IActionResult> ChangePassword(ChangePasswordVm vm) {
        if (!ModelState.IsValid) return View(vm);
        var (ok, body) = await _api.PutAsync("Auth/Change-password", new { vm.OldPassword, vm.NewPassword });
        if (!ok) { 
            vm.ErrorMessage = body.Contains("Incorrect old password") ? "Mật khẩu cũ không chính xác" : "Đổi mật khẩu thất bại";
            return View(vm); 
        }
        TempData["Success"] = "Đổi mật khẩu thành công!";
        return RedirectToAction("Index", "Home");
    }
}

public class AuthResponseDto {
    public string Token { get; set; } = "";
    public string Role { get; set; } = "";
    public string FullName { get; set; } = "";
    public int UserId { get; set; }
}
