using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using IceCreamProject.Models;
using IceCreamProject.ViewModels;
using IOFile = System.IO.File;

namespace IceCreamProject.Areas.System.Controllers
{
    [Area("System")]
    [Route("System/Books")]
    public class Books1Controller : Controller
    {
        private readonly ShopContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public Books1Controller(ShopContext context)
        {
            _context = context;
        }

        // GET: System/Books1
        [HttpGet("Index")]
        public async Task<IActionResult> Index()
        {
            var shopContext = _context.Books.Include(b => b.Category);
            return View(await shopContext.ToListAsync());
        }





        // GET: System/Books1/Create
        [HttpGet("Create")]
        public IActionResult Create()
        {
            ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "CategoryId");
            return View();
        }

        [HttpPost("Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create( BookViewModel book)
        {
            if (ModelState.IsValid)
            {
                // Xử lý upload ảnh
                string imagePath = string.Empty;
                if (book.ImageUrl != null)
                {
                    // Lấy tên file và lưu vào thư mục wwwroot/images
                    string fileName = Path.GetFileNameWithoutExtension(book.ImageUrl.FileName); //lấy tên file ảnh upload
                    string extension = Path.GetExtension(book.ImageUrl.FileName); //lấy định dạng file ảnh
                    imagePath = fileName + "_" + Guid.NewGuid().ToString() + extension; // tên file ảnh + chuỗi ngẫu nhiên + định dạng tấm ảnh => tên ảnh + 62521 + .jpg
                    string filePath = Path.Combine(_webHostEnvironment.WebRootPath, "ImageUrl", imagePath); //wwwroot/images/tên ảnh + 62521 + .jpg


                    // Lưu ảnh vào thư mục
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await book.ImageUrl.CopyToAsync(stream);
                    }
                }

                // Tạo đối tượng Product từ Viewbook
                var books = new Book
                {
                    Title = book.Title,
                    Description = book.Description,
                    Price = book.Price,
                    ImageUrl = imagePath,
                    CategoryId = book.CategoryId,
                };

                _context.Books.Add(books);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "CategoryId", book.CategoryId);
            return View(book);
        }






        // GET: System/Books1/Edit/5
        [HttpGet("Edit/{id:int}")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var book = await _context.Books.FindAsync(id);
            if (book == null)
            {
                return NotFound();
            }
            ViewBag.Categories = _context.Categories.ToList();
            return View(book);
        }

        [HttpPost]
        [HttpGet("Update/{id:int}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("BookId,Title,Description,Price,ImageUrl,CategoryId")] Book book)
        {
            if (id != book.BookId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(book);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BookExists(book.BookId))
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
            ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "CategoryId", book.CategoryId);
            return View(book);
        }

        // GET: System/Books1/Delete/5
        [HttpGet("Delete/{id:int}")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var book = await _context.Books
                .Include(b => b.Category)
                .FirstOrDefaultAsync(m => m.BookId == id);
            if (book == null)
            {
                return NotFound();
            }

            return View(book);
        }

        // POST: System/Books1/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var book = await _context.Books.FindAsync(id);
            if (book != null)
            {
                _context.Books.Remove(book);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BookExists(int id)
        {
            return _context.Books.Any(e => e.BookId == id);
        }
    }
}
