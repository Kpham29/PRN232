using System.Diagnostics;
using LMS.Application.DTOs;
using LMS.Web.Models;
using LMS.Web.Services;
using Microsoft.AspNetCore.Mvc;

namespace LMS.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApiService _api;
        public HomeController(ApiService api) => _api = api;

        public IActionResult Index() => View();

        public async Task<IActionResult> Dashboard() {
            var stats    = await _api.GetAsync<StatisticsVm>("Reports/statistics");
            var overdue  = await _api.GetListAsync<BorrowSlipVm>("Reports/overdue");
            var topBooks = await _api.GetListAsync<TopBookVm>("Reports/top-books?top=10");

            return View(new DashboardVm {
                Stats = stats ?? new(),
                OverdueBooks = overdue,
                TopBooks = topBooks
            });
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
