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
			var books = await _context.Books
				.Include(b => b.Category) // Gọi navigation property để lấy CategoryName
				.ToListAsync();

			return View(books);
		}




		// GET: System/Books1/Create
		[HttpGet("Create")]
		public IActionResult Create()
		{
			var viewModel = new BookViewModel
			{
				Categories = _context.Categories.Select(c => new SelectListItem
				{
					Value = c.CategoryId.ToString(),
					Text = c.Name // Lấy tên từ bảng Category
				}).ToList()
			};
			return View(viewModel);
		}


		[HttpPost("Create")]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Create(BookViewModel viewModel)
		{
			if (ModelState.IsValid)
			{
				string imagePath = string.Empty;

				// Xử lý upload ảnh
				if (viewModel.ImageUrl != null)
				{
					string fileName = Path.GetFileNameWithoutExtension(viewModel.ImageUrl.FileName);
					string extension = Path.GetExtension(viewModel.ImageUrl.FileName);
					imagePath = fileName + "_" + Guid.NewGuid().ToString() + extension;
					string filePath = Path.Combine(_webHostEnvironment.WebRootPath, "ImageUrl", imagePath);

					using (var stream = new FileStream(filePath, FileMode.Create))
					{
						await viewModel.ImageUrl.CopyToAsync(stream);
					}
				}

				// Lưu thông tin sách
				var book = new Book
				{
					Title = viewModel.Title,
					Description = viewModel.Description,
					Price = viewModel.Price,
					ImageUrl = imagePath,
					CategoryId = viewModel.CategoryId // Lưu CategoryId
				};

				_context.Books.Add(book);
				await _context.SaveChangesAsync();
				return RedirectToAction(nameof(Index));
			}

			// Nạp lại dropdown nếu ModelState không hợp lệ
			viewModel.Categories = _context.Categories.Select(c => new SelectListItem
			{
				Value = c.CategoryId.ToString(),
				Text = c.Name
			}).ToList();

			return View(viewModel);
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
