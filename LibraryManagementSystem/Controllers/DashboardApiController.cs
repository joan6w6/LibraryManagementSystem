using Microsoft.AspNetCore.Mvc;
using LibraryManagementSystem.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

[ApiController]
[Route("api/dashboard")]
public class DashboardApiController : ControllerBase
{
    private readonly LibraryContext _context;

    public DashboardApiController(LibraryContext context)
    {
        _context = context;
    }

    // API: 借閱統計數據
    [HttpGet("borrow-stats")]
    [Authorize(Roles = "Admin")] // 僅限管理員角色
    public async Task<IActionResult> GetBorrowStats([FromQuery] string period = "daily")
    {
        var query = _context.Transactions.AsQueryable();

        // 根據 period 提供不同的分組統計方式
        var stats = period.ToLower() switch
        {
            "monthly" => await query
                .GroupBy(t => new { t.BorrowedDate.Year, t.BorrowedDate.Month })
                .Select(g => new
                {
                    Period = $"{g.Key.Year}-{g.Key.Month:00}",
                    BorrowedCount = g.Count(t => t.ReturnedDate == null),
                    ReturnedCount = g.Count(t => t.ReturnedDate != null)
                })
                .ToListAsync(),
            _ => await query
                .GroupBy(t => t.BorrowedDate.Date)
                .Select(g => new
                {
                    Period = g.Key.ToString("yyyy-MM-dd"),
                    BorrowedCount = g.Count(t => t.ReturnedDate == null),
                    ReturnedCount = g.Count(t => t.ReturnedDate != null)
                })
                .ToListAsync()
        };


        return Ok(stats); // 返回 JSON 格式的數據
    }

    //API:根據借閱次數推薦熱門書籍
    [HttpGet("recommend-books")]
    public async Task<IActionResult> GetRecommendBooks()
    {
        var mostBorrowedBooks = await _context.Transactions
            .GroupBy(t => t.BookId)
            .Select(group => new
            {
                BookId = group.Key,
                BorrowCount = group.Count()
            })
            .OrderByDescending(g => g.BorrowCount)
            .Take(5)
            .Join(_context.Books,
                  g => g.BookId,
                  b => b.Id,
                  (g, b) => new
                  {
                      b.Title,
                      b.Author,
                      b.CoverImagePath,
                      g.BorrowCount
                  })
            .ToListAsync();

        return Ok(mostBorrowedBooks);
    }

    //API:查詢即將到期的書籍
    [HttpGet("due-books")]
    [Authorize] // 需要登入才能訪問
    public async Task<IActionResult> GetDueBooks()
    {
        // 取得當前用戶 ID
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

        // 獲取即將到期和已到期的書籍
        var dueBooks = await _context.Transactions
            .Include(t => t.Book) // 加載書籍信息
            .Where(t => t.UserId == userId && // 僅限當前用戶
                        t.ReturnedDate == null &&
                        (t.DueDate <= DateTime.Now.AddDays(3) || // 即將到期
                         t.DueDate < DateTime.Now)) // 已到期
            .Select(t => new
            {
                t.Book.Title,
                t.Book.Author,
                t.DueDate,
                t.ReturnedDate
            })
            .ToListAsync();

        return Ok(dueBooks); // 返回 JSON 格式的數據
       
    }

}
