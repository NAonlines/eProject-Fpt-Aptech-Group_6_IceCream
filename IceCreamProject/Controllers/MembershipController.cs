using IceCreamProject.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace IceCreamProject.Controllers
{
   
    public class MembershipController : Controller
	{
		private readonly ShopContext _db;
		private readonly UserManager<User> _userManager;
		private readonly IWebHostEnvironment _webHostEnvironment;
		public MembershipController(ShopContext db, UserManager<User> userManager, IWebHostEnvironment webHostEnvironment)
		{
			_db = db;
			_userManager = userManager;
			_webHostEnvironment = webHostEnvironment;
		}

		[HttpGet("payment-membership",Name = "Membership")]
		public async Task<IActionResult> Index()
		{

			var package = await _db.MemberPrice.OrderByDescending(x => x.CreateDate).ToListAsync();
			var user = await _userManager.GetUserAsync(User);
			if (user != null)
			{
				var checkMember = await _db.Memberships.Where(x => x.UserID == user.Id).ToListAsync();
				ViewBag.DataOrder = checkMember;

			}
			return View(package);
		}

		[HttpGet("recipes-user",Name = "RecipesUser")]
		public async Task<IActionResult> RecipesUser()
		{
			var user = await _userManager.GetUserAsync(User);
			if (user == null)
			{
				return Redirect("/login");
			}
			var checkMembership = await _db.Memberships.Where(x => x.UserID == user.Id && x.Status).ToListAsync();
			if (checkMembership.Count > 0)
			{
				foreach (var membership in checkMembership)
				{
					if (membership.EndDate <= DateTime.UtcNow)
					{
						membership.Status = false;
					}
				}
				await _db.SaveChangesAsync();
				var freeCategory = await _db.Categories.FirstOrDefaultAsync(c => c.Name == "Payment" && c.IsActive);
				if (freeCategory == null)
				{
					return View(new List<Recipe>());
				}
				var recipes = await _db.Recipes
			   .Include(r => r.Book)
			   .Include(r => r.Category)
			   .Where(r => r.CategoryId == freeCategory.CategoryId)
			   .ToListAsync();
				return View(recipes);


			}

			return Redirect("/payment-membership");
		}
	}
}
