using IceCreamProject.Models;
using IceCreamProject.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace IceCreamProject.Areas.System.Controllers
{
    [Area("System")]
    [Route("System/[controller]")]

    public class CategoryController : Controller
	{
		private readonly ShopContext db;
		public CategoryController(ShopContext context) {
			db = context;
        }
		public  async Task<IActionResult> Index()
		{
			return View(await db.Categories.ToListAsync());
		}

		[HttpGet("Create")]
        public  async Task<IActionResult> Create()
        {
            var model = new CategoryViewModel
            {
                ParentCategories = await db.Categories
                    .Where(c => c.ParentCategoryId == null) 
                    .Select(c => new SelectListItem
                    {
                        Text = c.Name,                   
                        Value = c.CategoryId.ToString() 
                    })
                    .ToArrayAsync()
            };

            return View(model);
        }
        [HttpPost("Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult>Create(CategoryViewModel model)
        {
            bool isDuplicate = await db.Categories.AnyAsync(c => c.Name == model.Name);

            if (isDuplicate)
            {
                ModelState.AddModelError("Name", "Category name already exists.");
                return View(model);
            }
            if (ModelState.IsValid)
            {
                var category = new Category
                {
                    Name = model.Name,
                    Description = model.Description,
                    IsActive = model.IsActive,
                    ParentCategoryId = model.ParentCategoryId
                };

                db.Categories.Add(category);
                await db.SaveChangesAsync();
                TempData["Message"] = "Category created successfully.";
                return RedirectToAction("Index");
            }
            return View(model);

        }
        [HttpGet("Delete/{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var category = await db.Categories
                .Include(c => c.ChildCategories)
                .FirstOrDefaultAsync(c => c.CategoryId == id);

            if (category == null)
            {
                return NotFound();
            }

            if (category.ChildCategories.Any())
            {
                TempData["Message"] = "Cannot delete category with child categories.";
                return RedirectToAction("Index");
            }
            db.Categories.Remove(category);
            await db.SaveChangesAsync();
            TempData["Message"] = "Category deleted successfully.";
            return RedirectToAction("Index");
        }

        [HttpGet("Update/{id:int}")]
        public async Task<IActionResult> Update(int id)
        {
            var category = await db.Categories.FirstOrDefaultAsync(c => c.CategoryId == id);

            if (category == null)
            {
                TempData["Message"] = "Category not found.";
                return RedirectToAction("Index");
            }

            var model = new CategoryViewModel
            {
                CategoryId = category.CategoryId,
                Name = category.Name,
                Description = category.Description,
                IsActive = category.IsActive,
                ParentCategoryId = category.ParentCategoryId,
                ParentCategories = await db.Categories
                .Where(c => c.ParentCategoryId == null && c.CategoryId != id) 
                .Select(c => new SelectListItem
                {
                    Text = c.Name,
                    Value = c.CategoryId.ToString()
                })
                .ToArrayAsync()
            };


            return View(model);
        }
        [HttpPost("Update/{id:int}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(int id, CategoryViewModel model)
        {
            if (id != model.CategoryId)
            {
                TempData["Message"] = "Category wrong Id.";
                return RedirectToAction("Index");
            }

            if (model.ParentCategoryId == model.CategoryId)
            {
                ModelState.AddModelError("ParentCategoryId", "A category cannot be its own parent.");
            }

            if (ModelState.IsValid)
            {
                var category = await db.Categories.FirstOrDefaultAsync(c => c.CategoryId == id);

                if (category == null)
                {
                    TempData["Message"] = "Category cannot be found.";
                    return RedirectToAction("Index");
                }

                category.Name = model.Name;
                category.Description = model.Description;
                category.IsActive = model.IsActive;
                category.ParentCategoryId = model.ParentCategoryId;

                await db.SaveChangesAsync();

                TempData["Message"] = "Category updated successfully.";
                return RedirectToAction("Index");
            }


            model.ParentCategories = await db.Categories
            .Where(c => c.ParentCategoryId == null && c.CategoryId != model.CategoryId)
            .Select(c => new SelectListItem
            {
                Text = c.Name,
                Value = c.CategoryId.ToString()
            })
            .ToArrayAsync();

            return View(model);
        }




    }
}
