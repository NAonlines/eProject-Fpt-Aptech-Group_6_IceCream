using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using IceCreamProject.Models;
using IceCreamProject.ViewModels;
using IOFile = System.IO.File;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace IceCreamProject.Areas.System.Controllers
{
    [Area("System")]
    [Route("System/[controller]")]
    [Authorize(Roles = "Admin")]
    public class RecipeController : Controller
    {
        private readonly ShopContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public RecipeController(ShopContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        [HttpGet("", Name = "Recipe")]
        public async Task<IActionResult> Index(string search = null)
        {
            var recipesQuery = _context.Recipes
                .Include(r => r.CreatedBy)
                .Include(r => r.Book)
                .Include(r => r.Category)
                .AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                search = search.ToLower(); 
                recipesQuery = recipesQuery.Where(r =>
                    r.Name.ToLower().Contains(search) ||
                    r.Ingredients.ToLower().Contains(search) ||
                    (r.Book != null && r.Book.Title.ToLower().Contains(search)) ||
                    (r.Category != null && r.Category.Name.ToLower().Contains(search)));
            }

            var recipes = await recipesQuery.ToListAsync();

            ViewBag.SearchQuery = search;
            return View(recipes);
        }




        // GET: Recipes/Create
        [HttpGet("Create")]
        public IActionResult Create()
        {
            // 
            var usedBookIds = _context.Recipes.Select(r => r.BookId).ToHashSet(); // 
            var availableBooks = _context.Books
                .Where(b => !usedBookIds.Contains(b.BookId)) // 
                .Select(b => new SelectListItem
                {
                    Value = b.BookId.ToString(),
                    Text = b.Title
                }).ToList();

            var viewModel = new RecipeViewModel
            {
                Categories = _context.Categories.Select(c => new SelectListItem
                {
                    Value = c.CategoryId.ToString(),
                    Text = c.Name
                }).ToList(),
                Books = availableBooks //
            };
            return View(viewModel);
        }

        //

        [HttpPost("Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(RecipeViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                string imagePath = string.Empty;

                // Xử lý ảnh
                if (viewModel.ImageUrl != null)
                {
                    string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "Images");
                    if (!Directory.Exists(uploadsFolder))
                    {
                        Directory.CreateDirectory(uploadsFolder);
                    }

                    string fileName = Path.GetFileNameWithoutExtension(viewModel.ImageUrl.FileName);
                    string extension = Path.GetExtension(viewModel.ImageUrl.FileName);
                    imagePath = $"{fileName}_{Guid.NewGuid()}{extension}";

                    string filePath = Path.Combine(uploadsFolder, imagePath);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await viewModel.ImageUrl.CopyToAsync(stream);
                    }
                }

                var createdById = User.FindFirstValue(ClaimTypes.NameIdentifier); 
                if (string.IsNullOrEmpty(createdById))
                {
                    ModelState.AddModelError("", "Unable to determine the current user's ID.");
                    return View(viewModel);
                }


                var recipe = new Recipe
                {
                    Name = viewModel.Name,
                    Ingredients = viewModel.Ingredients,
                    ImageUrl = imagePath,
                    IsApproved = viewModel.IsApproved,
                    CategoryId = viewModel.CategoryId,
                    BookId = viewModel.BookId,
                    CreatedById = createdById // Gán ID của người dùng vào CreatedById
                };

                _context.Recipes.Add(recipe);
                await _context.SaveChangesAsync();

                TempData["Message"] = "Recipe created successfully.";
                return RedirectToAction(nameof(Index));
            }

            // Nếu có lỗi, tải lại danh sách danh mục và sách
            viewModel.Categories = _context.Categories.Select(c => new SelectListItem
            {
                Value = c.CategoryId.ToString(),
                Text = c.Name
            }).ToList();

            viewModel.Books = _context.Books.Select(b => new SelectListItem
            {
                Value = b.BookId.ToString(),
                Text = b.Title
            }).ToList();

            return View(viewModel);
        }






        [HttpGet("Edit/{id:int}")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("Index");
            }

            var recipe = await _context.Recipes
                .Include(r => r.Category)
                .Include(r => r.Book)
                .FirstOrDefaultAsync(r => r.RecipeId == id);
            if (recipe == null)
            {
                return RedirectToAction("Index");
            }

            var model = new RecipeViewModel
            {
                RecipeId = recipe.RecipeId,
                Name = recipe.Name,
                Ingredients = recipe.Ingredients,
                IsApproved = recipe.IsApproved,
                CategoryId = recipe.CategoryId,
                BookId = recipe.BookId,
                Categories = await _context.Categories.Select(c => new SelectListItem
                {
                    Value = c.CategoryId.ToString(),
                    Text = c.Name,
                    Selected = c.CategoryId == recipe.CategoryId
                }).ToListAsync(),
                Books = await _context.Books.Select(b => new SelectListItem
                {
                    Value = b.BookId.ToString(),
                    Text = b.Title,
                    Selected = b.BookId == recipe.BookId
                }).ToListAsync()
            };

            return View(model);
        }


        [HttpPost("Edit/{id:int}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, RecipeViewModel viewModel)
        {
            if (id != viewModel.RecipeId)
            {
                return RedirectToAction("Index");
            }

            if (ModelState.IsValid)
            {
                var recipe = await _context.Recipes.FindAsync(id);

                if (recipe == null)
                {
                    return RedirectToAction("Index");
                }

                string imagePath = recipe.ImageUrl;

                // Xử lý hình ảnh
                if (viewModel.ImageUrl != null)
                {
                    string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "RecipeImages");
                    if (!Directory.Exists(uploadsFolder))
                    {
                        Directory.CreateDirectory(uploadsFolder);
                    }

                    // Xóa ảnh cũ nếu tồn tại
                    if (!string.IsNullOrEmpty(recipe.ImageUrl))
                    {
                        string oldImagePath = Path.Combine(uploadsFolder, recipe.ImageUrl);
                        if (IOFile.Exists(oldImagePath))
                        {
                            IOFile.Delete(oldImagePath);
                        }
                    }

                    string fileName = Path.GetFileNameWithoutExtension(viewModel.ImageUrl.FileName);
                    string extension = Path.GetExtension(viewModel.ImageUrl.FileName);
                    imagePath = $"{fileName}_{Guid.NewGuid()}{extension}";
                    string filePath = Path.Combine(uploadsFolder, imagePath);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await viewModel.ImageUrl.CopyToAsync(stream);
                    }
                }

                recipe.Name = viewModel.Name;
                recipe.Ingredients = viewModel.Ingredients;
                recipe.IsApproved = viewModel.IsApproved;
                recipe.ImageUrl = imagePath;
                recipe.CategoryId = viewModel.CategoryId;
                recipe.BookId = viewModel.BookId; 

                _context.Recipes.Update(recipe);
                await _context.SaveChangesAsync();

                TempData["Note"] = "Cập nhật công thức thành công.";
                return RedirectToAction("Index");
            }

            viewModel.Categories = await _context.Categories.Select(c => new SelectListItem
            {
                Value = c.CategoryId.ToString(),
                Text = c.Name,
                Selected = c.CategoryId == viewModel.CategoryId
            }).ToListAsync();

            viewModel.Books = await _context.Books.Select(b => new SelectListItem
            {
                Value = b.BookId.ToString(),
                Text = b.Title,
                Selected = b.BookId == viewModel.BookId
            }).ToListAsync();

            return View(viewModel);
        }




        [HttpGet("Delete/{id:int}")]
        public async Task<IActionResult> Delete(int? id)
        {
            //
            var recipe = await _context.Recipes.FindAsync(id);
            if (recipe != null)
            {
             
                if (!string.IsNullOrEmpty(recipe.ImageUrl))
                {
                    string imagePath = Path.Combine(_webHostEnvironment.WebRootPath, "images", recipe.ImageUrl);
                    if (IOFile.Exists(imagePath)) 
                    {
                        IOFile.Delete(imagePath);
                    }
                }

                _context.Recipes.Remove(recipe);
                await _context.SaveChangesAsync();
                TempData["Note"] = "Delete recipe successfully";
                return RedirectToAction("Index");
            }
            else
            {
                TempData["Note"] = "Recipe not found";
                return RedirectToAction("Index");
            }
        }

    }
}
