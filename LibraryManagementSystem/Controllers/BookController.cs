using Microsoft.AspNetCore.Mvc;
using LibraryManagementSystem.Data;
using LibraryManagementSystem.Models;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

public class BookController : BaseController
{
    private readonly LibraryContext _context;

    public BookController(LibraryContext context)
    {
        _context = context;
    }

    // Read: 書籍列表
    public IActionResult Index(string searchQuery)
    {

        //// 獲取當前登入用戶的 ID、name
        //var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        //var userNameClaim = User.FindFirstValue(ClaimTypes.Name);

        ////if (string.IsNullOrEmpty(userIdClaim))
        ////{
        ////    // 如果沒有登入
        ////    ViewBag.Message = "沒登入";
        ////}
        //ViewBag.UserId = userIdClaim;
        //ViewBag.UserName = userNameClaim;

        // 準備查詢書籍
        var books = _context.Books.AsQueryable();

        // 如果提供了搜尋關鍵字，則進行過濾
        if (!string.IsNullOrEmpty(searchQuery))
        {
            books = books.Where(b =>
                b.Title.Contains(searchQuery) ||  // 書名中包含關鍵字
                b.Author.Contains(searchQuery) || // 作者名包含關鍵字
                b.ISBN.Contains(searchQuery));    // ISBN 包含關鍵字
        }

        // 將搜索關鍵字存到 ViewBag，方便視圖使用
        ViewBag.SearchQuery = searchQuery;

        // 返回篩選後的書籍列表
        return View(books.ToList());
    }

    // Create: 顯示新增書籍頁面
    [Authorize(Roles = "Admin")]
    public IActionResult Create()
    {
        return View();
    }

    // Create: 處理新增書籍請求
    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create(Book book, IFormFile CoverImage)
    {
        if (ModelState.IsValid)
        {
            // 確保 AvailableCopies 不大於 TotalCopies
            if (book.AvailableCopies > book.TotalCopies)
            {
                ModelState.AddModelError("AvailableCopies", "Available copies cannot exceed total copies.");
                return View(book);
            }

            // 更新狀態
            if (book.AvailableCopies == 0)
            {
                book.Status = "已借完";
            }
            else
            {
                book.Status = "可借閱";
            }

            // 處理圖片上傳
            if (CoverImage != null && CoverImage.Length > 0)
            {
                var fileName = Path.GetFileName(CoverImage.FileName);
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images", fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await CoverImage.CopyToAsync(stream);
                }

                book.CoverImagePath = "/images/" + fileName;  // 存儲圖片路徑
            }


            _context.Books.Add(book);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }
        return View(book);
    }

    // Update: 顯示更新書籍頁面
    [Authorize(Roles = "Admin")]
    public IActionResult Edit(int id)
    {
        var book = _context.Books.Find(id);
        if (book == null) return NotFound();
        return View(book);
    }

    // Update: 處理更新書籍請求
    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Edit(int id, Book book, IFormFile CoverImage)
    {
        if (id != book.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            // 確保 AvailableCopies 不大於 TotalCopies
            if (book.AvailableCopies > book.TotalCopies)
            {
                ModelState.AddModelError("AvailableCopies", "Available copies cannot exceed total copies.");
                return View(book); 
            }

            // 更新狀態
            if (book.AvailableCopies == 0)
            {
                book.Status = "已借出";
            }
            else
            {
                book.Status = "可借閱";
            }

           
            // 處理圖片上傳
            if (CoverImage != null && CoverImage.Length > 0)
            {
                var fileName = Path.GetFileName(CoverImage.FileName);
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images", fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await CoverImage.CopyToAsync(stream);
                }

                book.CoverImagePath = "/images/" + fileName;  // 存儲圖片路徑
            }


            try
            {
                _context.Update(book);
                _context.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Books.Any(e => e.Id == book.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToAction(nameof(Index));
        }

        return View(book);
    }

    // Delete: 刪除書籍
    [Authorize(Roles = "Admin")]
    public IActionResult DeleteConfirmed(int id)
    {
        var book = _context.Books.Find(id);
        if (book != null)
        {
            _context.Books.Remove(book);
            _context.SaveChanges();
        }
        return RedirectToAction(nameof(Index));
    }
}
