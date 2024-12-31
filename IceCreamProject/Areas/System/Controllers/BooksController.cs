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
using Microsoft.AspNetCore.Authorization;

namespace IceCreamProject.Areas.System.Controllers
{
    [Area("System")]
    [Route("System/[controller]")]
    [Authorize(Roles = "Admin")]
    public class BooksController : Controller
    {
        private readonly ShopContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public BooksController(ShopContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        // GET: System/Books1/Index
        [HttpGet("", Name = "Book")]
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

        // POST: System/Books1/Create
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
                    // Tạo đường dẫn thư mục và tệp
                    string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "ImageUrl");
                    if (!Directory.Exists(uploadsFolder))
                    {
                        Directory.CreateDirectory(uploadsFolder); // Tạo thư mục nếu chưa tồn tại
                    }

                    string fileName = Path.GetFileNameWithoutExtension(viewModel.ImageUrl.FileName);
                    string extension = Path.GetExtension(viewModel.ImageUrl.FileName);
                    imagePath = $"{fileName}_{Guid.NewGuid()}{extension}";

                    string filePath = Path.Combine(uploadsFolder, imagePath);

                    // Lưu tệp vào thư mục
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await viewModel.ImageUrl.CopyToAsync(stream);
                    }
                }

                // Lưu thông tin sách vào cơ sở dữ liệu
                var book = new Book
                {
                    Title = viewModel.Title,
                    Description = viewModel.Description,
                    Price = viewModel.Price,
                    ImageUrl = imagePath, // Đường dẫn tương đối
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



        [HttpGet("Delete/{id:int}")]
        public async Task<IActionResult> Delete(int? id)
        {
            var product = await _context.Books.FindAsync(id);
            if (product != null)
            {
                if (!string.IsNullOrEmpty(product.ImageUrl))
                {
                    string imagePath = Path.Combine(_webHostEnvironment.WebRootPath, "images", product.ImageUrl);
                    if (IOFile.Exists(imagePath))
                    {
                        IOFile.Delete(imagePath);
                    }
                }

                _context.Books.Remove(product);
                await _context.SaveChangesAsync();
                TempData["Note"] = "Delete book successfully";
                return RedirectToAction("Index");
            }
            else
            {
                return RedirectToAction("Index");
            }

        }




        [HttpGet("Edit/{id:int}")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("Index");
            }

            var book = await _context.Books.Include(b => b.Category).FirstOrDefaultAsync(b => b.BookId == id);
            if (book == null)
            {
                return RedirectToAction("Index");
            }

            var model = new BookViewModel
            {
                BookId = book.BookId,
                Title = book.Title,
                Description = book.Description,
                Price = book.Price,
                CategoryId = book.CategoryId,
            };

            model.Categories = await _context.Categories.Select(c => new SelectListItem
            {
                Value = c.CategoryId.ToString(),
                Text = c.Name,
                Selected = c.CategoryId == book.CategoryId
            }).ToListAsync();

            return View(model);
        }





        [HttpPost("Edit/{id:int}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, BookViewModel viewModel)
        {
            if (id != viewModel.BookId)
            {
                return RedirectToAction("Index");
            }

            if (ModelState.IsValid)
            {
                var book = await _context.Books.FindAsync(id);

                if (book == null)
                {
                    return RedirectToAction("Index");
                }

                string imagePath = book.ImageUrl;

                // Xử lý cập nhật ảnh nếu có
                if (viewModel.ImageUrl != null)
                {
                    string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "ImageUrl");
                    if (!Directory.Exists(uploadsFolder))
                    {
                        Directory.CreateDirectory(uploadsFolder);
                    }

                    // Xóa ảnh cũ nếu có
                    if (!string.IsNullOrEmpty(book.ImageUrl))
                    {
                        string oldImagePath = Path.Combine(uploadsFolder, book.ImageUrl);
                        if (IOFile.Exists(oldImagePath))
                        {
                            IOFile.Delete(oldImagePath);
                        }
                    }

                    // Tạo đường dẫn ảnh mới
                    string fileName = Path.GetFileNameWithoutExtension(viewModel.ImageUrl.FileName);
                    string extension = Path.GetExtension(viewModel.ImageUrl.FileName);
                    imagePath = $"{fileName}_{Guid.NewGuid()}{extension}";
                    string filePath = Path.Combine(uploadsFolder, imagePath);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await viewModel.ImageUrl.CopyToAsync(stream);
                    }
                }

                // Cập nhật thông tin sách
                book.Title = viewModel.Title;
                book.Description = viewModel.Description;
                book.Price = viewModel.Price;
                book.ImageUrl = imagePath;
                book.CategoryId = viewModel.CategoryId;

                _context.Books.Update(book);
                await _context.SaveChangesAsync();

                TempData["Note"] = "Update book successfully";
                return RedirectToAction("Index");
            }

            // Nạp lại dropdown nếu ModelState không hợp lệ
            viewModel.Categories = await _context.Categories.Select(c => new SelectListItem
            {
                Value = c.CategoryId.ToString(),
                Text = c.Name,
                Selected = c.CategoryId == viewModel.CategoryId
            }).ToListAsync();

            return View(viewModel);
        }




    }

}
