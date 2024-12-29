using IceCreamProject.Models;
using IceCreamProject.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using IceCreamProject.Extensions;
using Microsoft.EntityFrameworkCore;
using System.Net;
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
			// Fetch the first 8 books from the database
			var books = _db.Books.Take(8).ToList();

			// Pass the books collection to the View
			return View(books);
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


        [HttpPost("/add-cart",Name ="AddCart")]
		public async Task<IActionResult> AddToCart(int BookId)
		{
			var book = await _db.Books.FirstOrDefaultAsync(b => b.BookId == BookId);

			if (book == null)
			{
				return Json(new { success = false, message = "Book not found." }); // Return JSON response();
			}
            //get session
            var cart = HttpContext.Session.GetObjectFromJson<List<CartItem>>("Cart") ?? new List<CartItem>();
            //check if book is already in cart

            var cartItem = cart.FirstOrDefault(p => p.ProductId == BookId);
			if (cartItem != null)
			{
                cartItem.Quantity++;
            }
            else
            {
				cart.Add(new CartItem
				{
					ProductId = BookId,
                    Name = book.Title,
					Price = book.Price,
					Image = book.ImageUrl,
					Quantity = 1
				});
            }
            //save to session

            HttpContext.Session.SetObjectAsJson("Cart", cart);
            int totalItems = cart.Sum(p => p.Quantity);
			return Json(new { success = true,totalItems, message = "Book added to cart." });
        }

        [HttpGet("/count-cart", Name = "CountCart")]
		public IActionResult GetCountCart()
		{
			var cart = HttpContext.Session.GetObjectFromJson<List<CartItem>>("Cart")?? new List<CartItem>();

            int totalItems = cart.Sum(p => p.Quantity);
			return Json(new { totalItems });
		}

        [HttpGet("/cart", Name = "Cart")]
        public IActionResult Cart()
		{
			var cart = HttpContext.Session.GetObjectFromJson<List<CartItem>>("Cart") ?? new List<CartItem>();
			return View(cart);
		}
	}
}
