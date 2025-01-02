using IceCreamProject.Models;
using IceCreamProject.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using IceCreamProject.Extensions;
using Microsoft.EntityFrameworkCore;
using System.Net;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Authorization;
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
			var books = _db.Books.Take(8).ToList();

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


		[HttpPost("/add-cart", Name = "AddCart")]
		public async Task<IActionResult> AddToCart(int BookId, int quantity)
		{
			var book = await _db.Books.FirstOrDefaultAsync(b => b.BookId == BookId);
			if (book == null)
			{
				return Json(new { success = false, message = "Book not found." });
			}

			// Get session
			var cart = HttpContext.Session.GetObjectFromJson<List<CartItem>>("Cart") ?? new List<CartItem>();

			// Check if the book is already in the cart
			var cartItem = cart.FirstOrDefault(p => p.ProductId == BookId);
			if (cartItem != null)
			{
				cartItem.Quantity += quantity; // Update quantity based on input
			}
			else
			{
				cart.Add(new CartItem
				{
					ProductId = BookId,
					Name = book.Title,
					Price = book.Price,
					Image = book.ImageUrl,
					Quantity = quantity
				});
			}

			// Save to session
			HttpContext.Session.SetObjectAsJson("Cart", cart);
			int totalItems = cart.Sum(p => p.Quantity);
			return Json(new { success = true, totalItems, message = "Book added to cart." });
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

            decimal totalPrice = cart.Sum(p => p.Quantity * p.Price);

            decimal totalWithShipping = totalPrice + 5;

            ViewData["TotalPrice"] = totalPrice; 
            ViewData["TotalWithShipping"] = totalWithShipping; 

            return View(cart);
        }



        [HttpPost("/remove-from-cart", Name = "RemoveFromCart")]
        public IActionResult RemoveFromCart(int productId)
        {
            var cart = HttpContext.Session.GetObjectFromJson<List<CartItem>>("Cart") ?? new List<CartItem>();
            var item = cart.FirstOrDefault(p => p.ProductId == productId);
            if (item != null)
            {
                cart.Remove(item);
            }
            HttpContext.Session.SetObjectAsJson("Cart", cart);
            return RedirectToAction("Cart");
        }
        [HttpPost("/update-cart", Name = "UpdateCart")]
        public IActionResult UpdateCart(int productId, int quantity)
        {
            var cart = HttpContext.Session.GetObjectFromJson<List<CartItem>>("Cart") ?? new List<CartItem>();
            var item = cart.FirstOrDefault(p => p.ProductId == productId);
            if (item != null)
            {
                item.Quantity = quantity;
            }
            HttpContext.Session.SetObjectAsJson("Cart", cart);
            return RedirectToAction("Cart");
        }
        [HttpGet("/cart-info", Name = "CartInfo")]
        public IActionResult GetCartInfo()
        {
            var cart = HttpContext.Session.GetObjectFromJson<List<CartItem>>("Cart") ?? new List<CartItem>();

            if (cart.Any())
            {
                return Json(new
                {
                    success = true,
                    totalItems = cart.Sum(p => p.Quantity),
                    cartItems = cart
                });
            }
            else
            {
                return Json(new { success = false, message = "Cart is empty." });
            }
        }


        [Authorize]
        [HttpGet("/check-out", Name = "CheckOut")]
        public async Task<IActionResult> CheckOut()
        {
            var cart = HttpContext.Session.GetObjectFromJson<List<CartItem>>("Cart") ?? new List<CartItem>();

            if (cart == null || !cart.Any())
            {
                TempData["Error"] = "Your cart is empty.";
                return RedirectToAction("Cart");
            }

            var model = new CheckoutViewModel
            {
                CartItems = cart,
                TotalAmount = cart.Sum(item => item.Quantity * item.Price) + 5,
                ShippingCost = 5 // Giá trị mặc định
            };

            return View(model);
        }






        [Authorize]
        [HttpPost("/check-out", Name = "CheckOut")]
        public async Task<IActionResult> CheckOut(CheckoutViewModel checkoutViewModel)
        {
            if (!ModelState.IsValid)
            {
                return View(checkoutViewModel);
            }

            var cart = HttpContext.Session.GetObjectFromJson<List<CartItem>>("Cart") ?? new List<CartItem>();

            if (cart.Any())
            {
                var currentUser = await _userManager.GetUserAsync(User);
                if (currentUser == null)
                {
                    return RedirectToAction("Login", "Account");
                }

                var newOrder = new Order
                {
                    CustomerName = currentUser.UserName,
                    Address = checkoutViewModel.ShippingAddress,
                    PhoneNumber = checkoutViewModel.PhoneNumber,
                    TotalAmount = cart.Sum(p => p.Quantity * p.Price) + checkoutViewModel.ShippingCost,
                    PaymentMethod = checkoutViewModel.PaymentMethod,
                    OrderDate = DateTime.UtcNow,
                    UserId = currentUser.Id,
                    OrderStatus = "Completed"
                };

                _db.Orders.Add(newOrder);
                await _db.SaveChangesAsync();

                foreach (var item in cart)
                {
                    var newCartItem = new CartItem
                    {
                        ProductId = item.ProductId,
                        Name = item.Name,
                        Price = item.Price,
                        Quantity = item.Quantity,
                        Image = item.Image,
                        OrderId = newOrder.OrderId
                    };

                    _db.CartItems.Add(newCartItem);
                }

                await _db.SaveChangesAsync();

                HttpContext.Session.Remove("Cart");

                TempData["Success"] = "Your order has been placed successfully!";
                return RedirectToAction("OrderConfirmation", new { orderId = newOrder.OrderId });
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }


        [Authorize]
        public IActionResult OrderConfirmation(int orderId)
        {
            var order = _db.Orders.Include(o => o.CartItems).FirstOrDefault(o => o.OrderId == orderId);
            if (order == null)
            {
                return RedirectToAction("Index", "Home");
            }
            return View(order);
        }

    }
}
