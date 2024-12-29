using LibraryManagementSystem.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Security.Claims;

namespace LibraryManagementSystem.Controllers
{
    public class HomeController : BaseController
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {

            // 獲取當前登入用戶的 ID
            //var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            //var userNameClaim = User.FindFirstValue(ClaimTypes.Name);
            //if (string.IsNullOrEmpty(userIdClaim))
            //{
                //如果沒有登入
            //    ViewBag.Message = "沒登入";
            //}
            //ViewBag.UserId = userIdClaim;
            //ViewBag.UserName = userNameClaim;
            return View();
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
