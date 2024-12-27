using IceCreamProject.Models;
using IceCreamProject.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace IceCreamProject.Controllers
{
    public class HomeController : Controller
    {
        private readonly ShopContext _db;
        private readonly UserManager<User> _userManager;

        public HomeController(ShopContext db, UserManager<User> userManager)
        {
            _db = db;
            _userManager = userManager;
        }

        [HttpGet("/", Name = "Home")]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet("/about-us", Name = "AboutUs")]
        public IActionResult AboutUs()
        {
            return View();
        }

        [HttpGet("/contact-us", Name = "ContactUs")]
        public async Task<IActionResult> ContactUs()
        {
            // Pre-fill user details if logged in
            var model = new FeedbackViewModel();
            if (User.Identity.IsAuthenticated)
            {
                var user = await _userManager.GetUserAsync(User);
                model.Email = user?.Email;
                model.Name = user?.UserName;
            }

            return View(model);
        }

        [HttpPost("/contact-us")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ContactUs(FeedbackViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Create Feedback instance
            var feedback = new Feedback
            {
                Name = model.Name, 
                Content = model.Message,
                SubmittedDate = DateTime.UtcNow,
                Email = model.Email, 
                UserId = User.Identity.IsAuthenticated ? _userManager.GetUserId(User) : null,
                User = User.Identity.IsAuthenticated ? await _userManager.GetUserAsync(User) : null
            };

            _db.Feedbacks.Add(feedback);
            await _db.SaveChangesAsync();

            // Notify user
            TempData["Success"] = "Thank you for your feedback! We will get back to you soon.";
            return RedirectToAction("ContactUs");
        }

    }
}
