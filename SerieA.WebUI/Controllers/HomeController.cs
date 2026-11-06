using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using SerieA.WebUI.Models;
using SerieA.WebUI.Services.Dashboard;
using SerieA.WebUI.ViewModels;

namespace SerieA.WebUI.Controllers
{
    public class HomeController(IDashboardApiService _dashboardApiService) : Controller
    {
        [HttpGet]
        [Authorize]
        [Route("Admin")]
        [Route("Home")]
        [Route("Home/Index")]
        public async Task<IActionResult> Index(CancellationToken cancellationToken)
        {
            var result = await _dashboardApiService.GetDashboardSummaryAsync(cancellationToken);
            var model = result.IsSuccess && result.Data != null
                ? new DashboardViewModel(result.Data)
                : new DashboardViewModel();
            return View(model);
        }

        [HttpGet]
        [AllowAnonymous]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            var exceptionHandlerPathFeature = HttpContext.Features.Get<IExceptionHandlerPathFeature>();
            var errorModel = new ErrorViewModel
            {
                RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier,
                StatusCode = 500,
                Message = "Beklenmedik bir hata oluştu. Sunucu isteği işlerken bir sorunla karşılaştı.",
                ExceptionMessage = exceptionHandlerPathFeature?.Error?.Message
            };

            return View(errorModel);
        }

        [HttpGet]
        [AllowAnonymous]
        [Route("Home/StatusCodeError")]
        public IActionResult StatusCodeError(int code)
        {
            var message = code switch
            {
                404 => "Aradığınız sayfa bulunamadı veya taşınmış olabilir.",
                401 => "Bu sayfayı görüntülemek için lütfen önce giriş yapınız.",
                403 => "Bu işlem veya sayfaya erişmek için yetkiniz bulunmuyor.",
                _ => "İstek işlenirken bir hata meydana geldi."
            };

            var errorModel = new ErrorViewModel
            {
                RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier,
                StatusCode = code,
                Message = message
            };

            return View("Error", errorModel);
        }
    }
}
