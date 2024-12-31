using IceCreamProject.Models;
using IceCreamProject.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace IceCreamProject.Areas.System.Controllers
{
    [Area("System")]
    [Route("System/[controller]")]
    [Authorize(Roles = "Admin")]
    public class UsersController : Controller
	{
		private readonly ShopContext db;
		public UsersController(ShopContext context) {
			db = context;
        }
        [HttpGet("", Name = "Users")]
		public  async Task<IActionResult> Index()
		{
			return View(await db.Users.ToListAsync());
		}

        [HttpPost("Delete")]
        public async Task<IActionResult> Delete(string userId)
        {
            var user = await db.Users.FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null)
            {
                return Json(new { success = false, message = "User not found." });
            }

            db.Users.Remove(user);
            await db.SaveChangesAsync();

            return Json(new { success = true, message = "User deleted successfully." });
        }



        [HttpGet("Update/{id}")]
        public async Task<IActionResult> Update(string id)
        {
            var user = await db.Users.FirstOrDefaultAsync(u => u.Id == id);
            if (user == null)
            {
                return NotFound();
            }

            var model = new UserViewModel
            {
                UserId = user.Id,
                Name = user.UserName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                Roles = await db.Roles.Select(r => new SelectListItem
                {
                    Value = r.Id,
                    Text = r.Name
                }).ToListAsync()
            };

            return View(model);
        }


        [HttpPost("Update")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(UserViewModel model)
        {

            var user = await db.Users.FirstOrDefaultAsync(u => u.Id == model.UserId);
            if (user == null)
            {
                return NotFound();
            }

            if (user.Email != model.Email)
            {
                bool isDuplicateEmail = await db.Users.AnyAsync(u => u.Email == model.Email);
                if (isDuplicateEmail)
                {
                    ModelState.AddModelError("Email", "The email address is already in use.");
                    model.Roles = await db.Roles.Select(r => new SelectListItem
                    {
                        Value = r.Id,
                        Text = r.Name
                    }).ToListAsync();
                    return View(model);
                }
            }

            user.UserName = model.Name;
            user.Email = model.Email;
            user.PhoneNumber = model.PhoneNumber;
            db.Users.Update(user);
            await db.SaveChangesAsync();
            return Json(new { success = true, message = "User updated successfully." });
        }
    }




}
