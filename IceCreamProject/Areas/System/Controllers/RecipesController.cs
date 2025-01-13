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

            if (!string.IsNullOrWhiteSpace(search))
            {
                search = search.ToLower();
                recipesQuery = recipesQuery.Where(r =>
                    r.Name.ToLower().Contains(search) ||
                    (r.Book != null && r.Book.Title.ToLower().Contains(search)) ||
                    (r.Category != null && r.Category.Name.ToLower().Contains(search)) ||
                    (r.CreatedBy != null && r.CreatedBy.UserName.ToLower().Contains(search))
                );
            }

            var recipes = await recipesQuery.ToListAsync();
            ViewBag.SearchQuery = search;
            return View(recipes);
        }

        [HttpGet("Create")]
        public IActionResult Create()
        {
            var viewModel = new RecipeViewModel
            {
                Categories = _context.Categories.Select(c => new SelectListItem
                {
                    Value = c.CategoryId.ToString(),
                    Text = c.Name
                }).ToList(),
                Books = _context.Books.Select(b => new SelectListItem
                {
                    Value = b.BookId.ToString(),
                    Text = b.Title
                }).ToList()
            };
            return View(viewModel);
        }

        [HttpPost("Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(RecipeViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                await LoadDropdownDataAsync(viewModel);
                return View(viewModel);
            }

            try
            {
                string imagePath = await SaveImageAsync(viewModel.ImageUrl);

                var createdById = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var recipe = new Recipe
                {
                    Name = viewModel.Name,
                    Ingredients = viewModel.Ingredients,
                    ImageUrl = imagePath,
                    IsApproved = viewModel.IsApproved,
                    CategoryId = viewModel.CategoryId,
                    BookId = viewModel.BookId,
                    CreatedById = createdById
                };

                _context.Recipes.Add(recipe);
                await _context.SaveChangesAsync();

                TempData["Message"] = "Recipe created successfully!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"An error occurred: {ex.Message}";
                await LoadDropdownDataAsync(viewModel);
                return View(viewModel);
            }
        }

        [HttpGet("Edit/{id:int}")]
        public async Task<IActionResult> Edit(int id)
        {
            var recipe = await _context.Recipes
                .Include(r => r.Category)
                .Include(r => r.Book)
                .FirstOrDefaultAsync(r => r.RecipeId == id);

            if (recipe == null) return NotFound();

            var viewModel = new RecipeViewModel
            {
                RecipeId = recipe.RecipeId,
                Name = recipe.Name,
                Ingredients = recipe.Ingredients,
                IsApproved = recipe.IsApproved,
                CategoryId = recipe.CategoryId,
                BookId = recipe.BookId,
                CreatedById = recipe.CreatedById,
                Categories = await GetCategoriesAsync(),
                Books = await GetBooksAsync()
            };

            return View(viewModel);
        }

        [HttpPost("Edit/{id:int}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, RecipeViewModel viewModel)
        {
            if (id != viewModel.RecipeId)
            {
                TempData["Error"] = "Invalid Recipe ID.";
                return RedirectToAction(nameof(Index));
            }

            if (!ModelState.IsValid)
            {
                await LoadDropdownDataAsync(viewModel);
                return View(viewModel);
            }

            try
            {
                var recipe = await _context.Recipes.FindAsync(id);
                if (recipe == null)
                {
                    TempData["Error"] = "Recipe not found.";
                    return RedirectToAction(nameof(Index));
                }

                recipe.Name = viewModel.Name;
                recipe.Ingredients = viewModel.Ingredients;
                recipe.IsApproved = viewModel.IsApproved;
                recipe.CategoryId = viewModel.CategoryId;
                recipe.BookId = viewModel.BookId;

                if (viewModel.ImageUrl != null)
                {
                    if (!string.IsNullOrEmpty(recipe.ImageUrl))
                    {
                        DeleteImage(recipe.ImageUrl);
                    }
                    recipe.ImageUrl = await SaveImageAsync(viewModel.ImageUrl);
                }

                _context.Recipes.Update(recipe);
                await _context.SaveChangesAsync();

                TempData["Message"] = "Recipe updated successfully!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"An error occurred: {ex.Message}";
                await LoadDropdownDataAsync(viewModel);
                return View(viewModel);
            }
        }

        [HttpGet("Delete/{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var recipe = await _context.Recipes.FindAsync(id);
                if (recipe == null)
                {
                    TempData["Error"] = "Recipe not found.";
                    return RedirectToAction(nameof(Index));
                }

                if (!string.IsNullOrEmpty(recipe.ImageUrl))
                {
                    DeleteImage(recipe.ImageUrl);
                }

                _context.Recipes.Remove(recipe);
                await _context.SaveChangesAsync();

                TempData["Message"] = "Recipe deleted successfully!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"An error occurred: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        private async Task<List<SelectListItem>> GetCategoriesAsync()
        {
            return await _context.Categories.Select(c => new SelectListItem
            {
                Value = c.CategoryId.ToString(),
                Text = c.Name
            }).ToListAsync();
        }

        private async Task<List<SelectListItem>> GetBooksAsync()
        {
            return await _context.Books.Select(b => new SelectListItem
            {
                Value = b.BookId.ToString(),
                Text = b.Title
            }).ToListAsync();
        }

        private async Task LoadDropdownDataAsync(RecipeViewModel viewModel)
        {
            viewModel.Categories = await GetCategoriesAsync();
            viewModel.Books = await GetBooksAsync();
        }

        private async Task<string> SaveImageAsync(IFormFile image)
        {
            if (image == null) return null;

            string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "ImageUrl");
            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            string fileName = $"{Path.GetFileNameWithoutExtension(image.FileName)}_{Guid.NewGuid()}{Path.GetExtension(image.FileName)}";
            string filePath = Path.Combine(uploadsFolder, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await image.CopyToAsync(stream);
            }

            return fileName;
        }

        private void DeleteImage(string imagePath)
        {
            string fullPath = Path.Combine(_webHostEnvironment.WebRootPath, "ImageUrl", imagePath);
            if (IOFile.Exists(fullPath))
            {
                IOFile.Delete(fullPath);
            }
        }
    }
}
