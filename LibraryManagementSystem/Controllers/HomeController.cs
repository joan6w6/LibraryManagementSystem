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

            // �����e�n�J�Τ᪺ ID
            //var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            //var userNameClaim = User.FindFirstValue(ClaimTypes.Name);
            //if (string.IsNullOrEmpty(userIdClaim))
            //{
                //�p�G�S���n�J
            //    ViewBag.Message = "�S�n�J";
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
