using Microsoft.AspNetCore.Mvc;
using LibraryManagementSystem.Data;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using LibraryManagementSystem.Models;
using Microsoft.EntityFrameworkCore;


public class UserController : BaseController
{
    private readonly LibraryContext _context;

    public UserController(LibraryContext context)
    {
        _context = context;
    }

    // 顯示登入頁面
    public IActionResult Login()
    {
        return View();
    }

    // 處理登入表單提交
    [HttpPost]
    public async Task<IActionResult> Login(string username, string password)
    {
        // 驗證用戶憑據
        var user = _context.Users.SingleOrDefault(u => u.Username == username);

        if (user == null) // 沒有找到該 username 的用戶
        {
            ViewBag.Message = "Username does not exist.";
            return View();
        }

        if (user.Password != password) // 密碼不正確
        {
            ViewBag.Message = "Incorrect password.";
            return View();
        }

    // 創建身份驗證的 Claims
    var claims = new List<Claim>
    {
        new Claim(ClaimTypes.NameIdentifier, user.UserID.ToString()), // 用戶 ID
        new Claim(ClaimTypes.Name, user.Username), // 用戶名
        new Claim(ClaimTypes.Role, user.Role) // 角色
    };

    // 創建 ClaimsIdentity
    var claimsIdentity = new ClaimsIdentity(claims, "Login");

    // 設置身份驗證 Cookie
    await HttpContext.SignInAsync(new ClaimsPrincipal(claimsIdentity));

        return RedirectToAction("Index", "Home");
    }

    public async Task<IActionResult> Logout()
    {
        // 清除身份驗證 Cookie
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

        // 重定向到首頁
        return RedirectToAction("Index", "Home");
    }

    // 用戶查看已借書籍
    public IActionResult ViewBorrowedBooks()
    {
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrEmpty(userIdClaim))
        {
            TempData["Message"] = "Please log in.";
            return RedirectToAction("Login", "User");
        }

        if (!int.TryParse(userIdClaim, out int userId))
        {
            return View("Error", new ErrorViewModel { RequestId = "Invalid User ID" });
        }

        var borrowedBooks = _context.Transactions
            .Include(t => t.Book)  // 加載書籍資料
            .Where(t => t.UserId == userId && t.ReturnedDate == null) // 只顯示未歸還的書籍
            .ToList();

        return View(borrowedBooks);
    }
}
