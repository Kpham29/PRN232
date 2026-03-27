using LMS.Web.Models;
using LMS.Web.Services;
using Microsoft.AspNetCore.Mvc;
using LMS.Web.Filters;

namespace LMS.Web.Controllers;

[SessionAuthorize(Roles = "Admin")]
public class CategoryController : Controller
{
    private readonly ApiService _api;
    public CategoryController(ApiService api) => _api = api;

    public async Task<IActionResult> Index()
    {
        var categories = await _api.GetListAsync<CategoryVm>("Categories");
        return View(categories);
    }

    public IActionResult Create() => View(new CategoryVm());

    [HttpPost]
    public async Task<IActionResult> Create(CategoryVm vm)
    {
        if (!ModelState.IsValid) return View(vm);
        var (ok, _) = await _api.PostAsync("Categories", new { vm.Name });
        if (!ok) { ModelState.AddModelError("", "Thêm thể loại thất bại"); return View(vm); }
        TempData["Success"] = "Thêm thể loại thành công!";
        return RedirectToAction("Index");
    }

    public async Task<IActionResult> Edit(int id)
    {
        var c = await _api.GetAsync<CategoryVm>($"Categories/{id}");
        if (c is null) return NotFound();
        return View(c);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(int id, CategoryVm vm)
    {
        if (!ModelState.IsValid) return View(vm);
        var (ok, _) = await _api.PutAsync($"Categories/{id}", new { vm.Name });
        if (!ok) { ModelState.AddModelError("", "Cập nhật thất bại"); return View(vm); }
        TempData["Success"] = "Cập nhật thành công!";
        return RedirectToAction("Index");
    }

    [HttpPost]
    public async Task<IActionResult> Delete(int id)
    {
        await _api.DeleteAsync($"Categories/{id}");
        TempData["Success"] = "Đã xóa thể loại.";
        return RedirectToAction("Index");
    }
}
