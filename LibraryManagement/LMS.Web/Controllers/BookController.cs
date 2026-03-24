using LMS.Web.Models;
using LMS.Web.Services;
using Microsoft.AspNetCore.Mvc;

namespace LMS.Web.Controllers;

public class BookController : Controller {
    private readonly ApiService _api;
    public BookController(ApiService api) => _api = api;

    public async Task<IActionResult> Index(string? search, int? categoryId, int page = 1) {
        const int size = 12;
        var f = new List<string>();
        if (!string.IsNullOrWhiteSpace(search))
            f.Add($"(contains(Title,'{search}') or contains(Author,'{search}'))");
        if (categoryId.HasValue) f.Add($"CategoryId eq {categoryId}");
        var q     = f.Any() ? "$filter=" + string.Join(" and ", f) + "&" : "";
        var books = await _api.GetListAsync<BookVm>($"Books?{q}$orderby=Title&$top={size}&$skip={(page-1)*size}");
        return View(new BookListVm {
            Books      = books,
            Categories = await _api.GetListAsync<CategoryVm>("Categories"),
            Search = search, CategoryId = categoryId, Page = page, HasNext = books.Count == size
        });
    }

    public async Task<IActionResult> Detail(int id) =>
        await _api.GetAsync<BookVm>($"Books/{id}") is { } b ? View(b) : NotFound();

    public async Task<IActionResult> Create() {
        var vm = new BookFormVm {
            Categories = await _api.GetListAsync<CategoryVm>("Categories"),
            Publishers = await _api.GetListAsync<PublisherVm>("Publishers")
        };
        return View(vm);
    }

    [HttpPost] public async Task<IActionResult> Create(BookFormVm vm) {
        if (!ModelState.IsValid) { await FillDropdowns(vm); return View(vm); }
        var (ok, _) = await _api.PostAsync("Books",
            new { vm.Title, vm.Author, vm.Price, vm.PublishedYear,
                  vm.ISBN, vm.Description, vm.CoverUrl, vm.TotalQuantity,
                  vm.CategoryId, vm.PublisherId });
        if (!ok) { ModelState.AddModelError("", "Thêm sách thất bại"); await FillDropdowns(vm); return View(vm); }
        TempData["Success"] = "Thêm sách thành công!";
        return RedirectToAction("Index");
    }

    public async Task<IActionResult> Edit(int id) {
        var b = await _api.GetAsync<BookVm>($"Books/{id}");
        if (b is null) return NotFound();
        var vm = new BookFormVm { Id=id, Title=b.Title, Author=b.Author, Price=b.Price,
            CategoryId=b.CategoryId, PublisherId=b.PublisherId, ISBN=b.ISBN,
            PublishedYear=b.PublishedYear, TotalQuantity=b.TotalQuantity,
            Description=b.Description, CoverUrl=b.CoverUrl };
        await FillDropdowns(vm);
        return View(vm);
    }

    [HttpPost] public async Task<IActionResult> Edit(int id, BookFormVm vm) {
        if (!ModelState.IsValid) { await FillDropdowns(vm); return View(vm); }
        await _api.PutAsync($"Books/{id}", new { vm.Title, vm.Author, vm.Price,
            vm.PublishedYear, vm.ISBN, vm.Description, vm.CoverUrl,
            vm.TotalQuantity, vm.CategoryId, vm.PublisherId });
        TempData["Success"] = "Cập nhật thành công!";
        return RedirectToAction("Index");
    }

    [HttpPost] public async Task<IActionResult> Delete(int id) {
        await _api.DeleteAsync($"Books/{id}");
        TempData["Success"] = "Đã xóa sách.";
        return RedirectToAction("Index");
    }

    [HttpPost] public async Task<IActionResult> RequestBorrow(int id, string? note) {
        var (ok, body) = await _api.PostAsync("BorrowRequests", new { BookId = id, Note = note });
        if (!ok) { 
            // The body might be a plain error message or a JSON object depending on how it's handled in the API
            TempData["Error"] = "Yêu cầu mượn thất bại: " + (string.IsNullOrWhiteSpace(body) ? "Lỗi không xác định" : body); 
        }
        else { TempData["Success"] = "Gửi yêu cầu mượn thành công! Vui lòng chờ thủ thư duyệt."; }
        return RedirectToAction("Detail", new { id });
    }

    private async Task FillDropdowns(BookFormVm vm) {
        vm.Categories = await _api.GetListAsync<CategoryVm>("Categories");
        vm.Publishers = await _api.GetListAsync<PublisherVm>("Publishers");
    }
}
