using LMS.Web.Models;
using LMS.Web.Services;
using Microsoft.AspNetCore.Mvc;
using LMS.Web.Filters;

namespace LMS.Web.Controllers;

[SessionAuthorize(Roles = "Admin")]
public class PublisherController : Controller
{
    private readonly ApiService _api;
    public PublisherController(ApiService api) => _api = api;

    public async Task<IActionResult> Index()
    {
        var publishers = await _api.GetListAsync<PublisherVm>("Publishers");
        return View(publishers);
    }

    public IActionResult Create() => View(new PublisherVm());

    [HttpPost]
    public async Task<IActionResult> Create(PublisherVm vm)
    {
        if (!ModelState.IsValid) return View(vm);
        var (ok, _) = await _api.PostAsync("Publishers", new { vm.Name, vm.Address, vm.Phone });
        if (!ok) { ModelState.AddModelError("", "Thêm nhà xuất bản thất bại"); return View(vm); }
        TempData["Success"] = "Thêm nhà xuất bản thành công!";
        return RedirectToAction("Index");
    }

    public async Task<IActionResult> Edit(int id)
    {
        var p = await _api.GetAsync<PublisherVm>($"Publishers/{id}");
        if (p is null) return NotFound();
        return View(p);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(int id, PublisherVm vm)
    {
        if (!ModelState.IsValid) return View(vm);
        var (ok, _) = await _api.PutAsync($"Publishers/{id}", new { vm.Name, vm.Address, vm.Phone });
        if (!ok) { ModelState.AddModelError("", "Cập nhật thất bại"); return View(vm); }
        TempData["Success"] = "Cập nhật thành công!";
        return RedirectToAction("Index");
    }

    [HttpPost]
    public async Task<IActionResult> Delete(int id)
    {
        await _api.DeleteAsync($"Publishers/{id}");
        TempData["Success"] = "Đã xóa nhà xuất bản.";
        return RedirectToAction("Index");
    }
}
