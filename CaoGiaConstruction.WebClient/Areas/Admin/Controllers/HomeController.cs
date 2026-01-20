using Microsoft.AspNetCore.Mvc;
using CaoGiaConstruction.WebClient.Services;

namespace CaoGiaConstruction.WebClient.Areas.Admin.Controllers
{
    public class HomeController : BaseController
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IReportService _reportService;

        public HomeController(ILogger<HomeController> logger, IReportService reportService)
        {
            _logger = logger;
            _reportService = reportService;
        }

        [Route("/admin", Name = "admin-default")]
        public async Task<IActionResult> Index()
        {
            var resultReport = await _reportService.GetStatisticalHome();
            return View(resultReport);
        }
    }
}