using LibraryManagementSystem.Data;
using Microsoft.AspNetCore.Mvc;
using LibraryManagementSystem.Models;

namespace LibraryManagementSystem.Controllers
{
    public class AccountController : Controller
    {
        private readonly LibraryContext _context;

        public AccountController(LibraryContext context)
        {
            _context = context;
        }


        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Register(string username, string password, string role = "User")
        {
            if (ModelState.IsValid)
            {
                // 檢查用戶名是否已存在
                if (_context.Users.Any(u => u.Username == username))
                {
                    ModelState.AddModelError("", "Username is already taken.");
                    return View();
                }

                // 新增用戶到資料庫
                var user = new User
                {
                    Username = username,
                    Password = password, // 建議加密密碼
                    Role = role
                };
                _context.Users.Add(user);
                var result = _context.SaveChanges();

                if (result > 0) // SaveChanges() 返回值為影響的行數
                {
                    TempData["Message"] = "Registration successful! Please log in.";
                    return RedirectToAction("Login", "User");
                }
              
            }

            return View();
        }


    }
}
