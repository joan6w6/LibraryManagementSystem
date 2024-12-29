using LibraryManagementSystem.Data;
using LibraryManagementSystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

public class TransactionController : BaseController
{
    private readonly LibraryContext _context;

    public TransactionController(LibraryContext context)
    {
        _context = context;
    }

    [HttpPost]
    public IActionResult Borrow(int bookId)
    {
        // 獲取當前登入用戶的 ID
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrEmpty(userIdClaim))
        {
            TempData["Message"] = " Please log in.";
            // 如果沒有登入，用戶 ID 為 null 或空，則重定向到登錄頁面或顯示錯誤
            return RedirectToAction("Login", "User");
        }

        if (!int.TryParse(userIdClaim, out int userId))
        {
            // 如果用戶 ID 解析失敗，返回錯誤
            return View("Error", new ErrorViewModel { RequestId = "Invalid User ID" });
        }

        var book = _context.Books.Find(bookId);

        if (book == null || book.AvailableCopies <= 0)
        {
            // 如果書籍不存在或沒有可借書籍，則重定向回書籍列表頁面
            return RedirectToAction("Index", "Book");
        }

        // 在這裡添加借書邏輯
        var transaction = new Transaction
        {
            UserId = userId,
            BookId = bookId,
            BorrowedDate = DateTime.Now,
            DueDate = DateTime.Now.AddDays(7) // 設定到期日期為借書後七天
        };

        _context.Transactions.Add(transaction);
        book.AvailableCopies--; // 更新書籍的可借數量
        _context.SaveChanges();
        TempData["Message"] = "Borrowed successful!";
        return RedirectToAction("ViewBorrowedBooks", "User"); 
    }
    [HttpPost]
    public IActionResult Return(int transactionId)
    {
        var transaction = _context.Transactions
                                  .FirstOrDefault(t => t.Id == transactionId);

        if (transaction == null || transaction.ReturnedDate != null)
        {
            TempData["Message"] = "Transaction not found or already returned.";
            return RedirectToAction("ViewBorrowedBooks", "User");
        }

        // 更新交易記錄為已歸還
        transaction.ReturnedDate = DateTime.Now;

        // 增加書籍的可借閱數量
        var book = _context.Books.FirstOrDefault(b => b.Id == transaction.BookId);
        if (book != null)
        {
            book.AvailableCopies++;
        }

        _context.SaveChanges();

        TempData["Message"] = "Book returned successfully!";
        return RedirectToAction("ViewBorrowedBooks", "User");
    }
}
