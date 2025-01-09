using IceCreamProject.Extensions;
using IceCreamProject.Models;
using IceCreamProject.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
namespace IceCreamProject.Controllers
{
    public class HomeController : Controller
    {
        private readonly ShopContext _db;
        private readonly UserManager<User> _userManager;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public HomeController(ShopContext db, UserManager<User> userManager, IWebHostEnvironment webHostEnvironment)
        {
            _db = db;
            _userManager = userManager;
            _webHostEnvironment = webHostEnvironment;
        }

		[HttpGet("/", Name = "Home")]
		public IActionResult Index()
		{
			// AllProduct
			var allProducts = _db.Books
								 .OrderBy(b => b.IsActive ? 0 : 1)
								 .Take(8)
								 .ToList();

			// Best Selling
			var bestSellingProducts = _db.CartItems
										 .GroupBy(c => c.ProductId)
										 .Select(g => new
										 {
											 ProductId = g.Key,
											 TotalQuantity = g.Sum(c => c.Quantity)
										 })
										 .OrderByDescending(g => g.TotalQuantity)
										 .Take(8)
										 .Join(_db.Books,
											   cartItem => cartItem.ProductId,
											   book => book.BookId,
											   (cartItem, book) => book)
										 .ToList();
            // New Product
			var newProducts = _db.Books
						 .OrderByDescending(b => b.BookId).ToList();

			var viewModel = new HomeViewModel
			{
                AllProducts = allProducts,
				NewProducts = newProducts,
				BestSellingProducts = bestSellingProducts
			};

			return View(viewModel);
		}

		[Authorize]
        [HttpGet("/profile", Name = "Profile")]
        public async Task<IActionResult> Profile()
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                TempData["Error"] = "User not found.";
                return RedirectToAction("Index", "Home");
            }

            var transactionHistory = await _db.Orders
                .Where(o => o.UserId == user.Id)
                .Include(o => o.CartItems)
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();

            var model = new ProfileViewModel
            {
                Email = user.Email,
                Phone = user.PhoneNumber,
                Address = user.Address,
                ProfileImageUrl = string.IsNullOrWhiteSpace(user.ProfileImageUrl) ? "/default-profile.png" : user.ProfileImageUrl,
                TransactionHistory = transactionHistory
            };

            return View(model);
        }


        [HttpPost]
        [Authorize]
        public async Task<IActionResult> UpdateProfile()
        {
            var phone = Request.Form["Phone"];
            var address = Request.Form["Address"];
            var profileImage = Request.Form.Files["ProfileImage"];
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                TempData["Error"] = "User not found.";
                return RedirectToAction("Index", "Home");
            }

            user.PhoneNumber = phone;
            user.Address = address;

            if (profileImage != null && profileImage.Length > 0)
            {
                var uploads = Path.Combine(_webHostEnvironment.WebRootPath, "Images");

                if (!Directory.Exists(uploads))
                {
                    Directory.CreateDirectory(uploads);
                }

                // Delete old profile image if it exists
                if (!string.IsNullOrEmpty(user.ProfileImageUrl) && user.ProfileImageUrl != "/default-profile.png")
                {
                    var oldFilePath = Path.Combine(_webHostEnvironment.WebRootPath, user.ProfileImageUrl.TrimStart('/'));
                    if (System.IO.File.Exists(oldFilePath))
                    {
                        System.IO.File.Delete(oldFilePath);
                    }
                }

                // Save the new profile image
                var uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(profileImage.FileName);
                var filePath = Path.Combine(uploads, uniqueFileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await profileImage.CopyToAsync(stream);
                }

                user.ProfileImageUrl = "/Images/" + uniqueFileName;
            }

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                TempData["Error"] = "Error updating profile.";
                return RedirectToAction("Profile");
            }

            TempData["Success"] = "Profile updated successfully!";
            return RedirectToAction("Profile");
        }


        [HttpPost]
        [Authorize]
        public async Task<IActionResult> ChangePassword(string currentPassword, string newPassword)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                TempData["Error"] = "User not found.";
                return RedirectToAction("Index", "Home");
            }

            if (string.IsNullOrEmpty(newPassword) || newPassword.Length < 6 || !newPassword.Any(char.IsUpper))
            {
                TempData["Error"] = "New password must be at least 6 characters long and contain at least one uppercase letter.";
                return RedirectToAction("Profile");
            }

            var passwordCheck = await _userManager.CheckPasswordAsync(user, newPassword);
            if (passwordCheck)
            {
                TempData["Error"] = "New password cannot be the same as the current password.";
                return RedirectToAction("Profile");
            }

            var result = await _userManager.ChangePasswordAsync(user, currentPassword, newPassword);
            if (!result.Succeeded)
            {
                TempData["Error"] = string.Join(" ", result.Errors.Select(e => e.Description));
                return RedirectToAction("Profile");
            }

            TempData["Success"] = "Password changed successfully!";
            return RedirectToAction("Profile");
        }






        [HttpGet("/product-details/{name}-{id}", Name = "ProductDetails")]
        public async Task<IActionResult> ProductDetails(int id)
        {
            var product = await _db.Books
                .Include(b => b.Category)
                .Include(b => b.Recipes)
                .ThenInclude(r => r.Category)  // Ensure Recipe's Category is loaded as well
                .FirstOrDefaultAsync(b => b.BookId == id && b.IsActive);

            if (product == null)
            {
                return Json(new { success = false, message = "This book is currently out of stock.", redirectUrl = Url.Action("Index", "Home") });
            }

            var user = await _userManager.GetUserAsync(User);
            bool hasValidMembership = false;

            if (user != null)
            {
                var membership = await _db.Memberships
                    .FirstOrDefaultAsync(m => m.UserID == user.Id && m.Status && m.EndDate > DateTime.UtcNow);

                hasValidMembership = membership != null;
            }

            ViewData["HasValidMembership"] = hasValidMembership;

            var paymentCategory = await _db.Categories
                .FirstOrDefaultAsync(c => c.Name == "Payment" && c.IsActive);

            if (paymentCategory != null)
            {
                var paymentRecipes = product.Recipes.Where(r => r.CategoryId == paymentCategory.CategoryId).ToList();
                ViewData["Recipes"] = paymentRecipes; // Only "Payment" category recipes
            }
            else
            {
                ViewData["Recipes"] = new List<Recipe>(); // If no "Payment" category exists, pass an empty list
            }

            return View(product);
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
			if (quantity <= 0)
			{
				return Json(new { success = false, message = "Invalid quantity." });
			}

			var book = await _db.Books.FirstOrDefaultAsync(b => b.BookId == BookId);
			if (book == null)
			{
				return Json(new { success = false, message = "Book not found." });
			}

			if (!book.IsActive)
			{
				return Json(new { success = false, message = "This book is currently out of stock." });
			}

			var cart = HttpContext.Session.GetObjectFromJson<List<CartItem>>("Cart") ?? new List<CartItem>();

			var cartItem = cart.FirstOrDefault(p => p.ProductId == BookId);
			if (cartItem != null)
			{
				cartItem.Quantity += quantity;
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

			HttpContext.Session.SetObjectAsJson("Cart", cart);
			int totalItems = cart.Sum(p => p.Quantity);
			return Json(new { success = true, totalItems, message = "Book added to cart successfully!" });
		}



		[HttpGet("/count-cart", Name = "CountCart")]
        public IActionResult GetCountCart()
        {
            var cart = HttpContext.Session.GetObjectFromJson<List<CartItem>>("Cart") ?? new List<CartItem>();

            int totalItems = cart.Sum(p => p.Quantity);
            return Json(new { totalItems });
        }

        [HttpGet("/cart", Name = "Cart")]
        public IActionResult Cart()
        {
            var cart = HttpContext.Session.GetObjectFromJson<List<CartItem>>("Cart") ?? new List<CartItem>();

            decimal totalPrice = cart.Sum(p => p.Quantity * p.Price);

            decimal totalWithShipping = 0;

            if (cart.Any())
            {
                totalWithShipping = totalPrice + 5;
            }
            else
            {
                totalWithShipping = totalPrice;
            }

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
                TempData["Success"] = "The product has been removed from your cart.";
            }
            else
            {
                TempData["Error"] = "Product not found in the cart.";
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
            if (!cart.Any())
            {
                TempData["Error"] = "Your cart is empty.";
                return RedirectToAction("Cart");
            }

            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                TempData["Error"] = "User information is missing.";
                return RedirectToAction("Login", "Account");
            }

            decimal shippingCost = 5;
            decimal totalAmount = cart.Sum(item => item.Quantity * item.Price) + shippingCost;

            var checkoutData = new CheckoutDTO
            {
                Cart = cart,
                User = currentUser,
                ShippingCost = shippingCost,
                TotalAmount = totalAmount
            };

            return View(checkoutData);
        }






        [Authorize]
        [HttpPost("/check-out", Name = "CheckOut")]
        public async Task<IActionResult> CheckOut(string shippingAddress, string phoneNumber, string paymentMethod)
        {
            var cart = HttpContext.Session.GetObjectFromJson<List<CartItem>>("Cart") ?? new List<CartItem>();

            if (cart == null || !cart.Any())
            {
                TempData["Error"] = "Your cart is empty.";
                return RedirectToAction("Cart");
            }

            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                TempData["Error"] = "User information is missing.";
                return RedirectToAction("Login", "Account");
            }

            if (string.IsNullOrWhiteSpace(shippingAddress) || string.IsNullOrWhiteSpace(phoneNumber))
            {
                TempData["Error"] = "Shipping address and phone number are required.";
                return RedirectToAction("CheckOut");
            }

            // Update user information
            currentUser.Address = shippingAddress;
            currentUser.PhoneNumber = phoneNumber;
            var updateResult = await _userManager.UpdateAsync(currentUser);

            if (!updateResult.Succeeded)
            {
                TempData["Error"] = "Failed to update user information.";
                return RedirectToAction("CheckOut");
            }

            decimal shippingCost = 5;
            decimal totalAmount = cart.Sum(item => item.Quantity * item.Price) + shippingCost;

            var newOrder = new Order
            {
                CustomerName = currentUser.UserName,
                Address = shippingAddress,
                PhoneNumber = phoneNumber,
                TotalAmount = totalAmount,
                PaymentMethod = paymentMethod,
                OrderDate = DateTime.UtcNow,
                UserId = currentUser.Id,
                OrderStatus = "Processing"
            };

            _db.Orders.Add(newOrder);
            await _db.SaveChangesAsync();

            foreach (var item in cart)
            {
                var orderDetail = new CartItem
                {
                    ProductId = item.ProductId,
                    Name = item.Name,
                    Price = item.Price,
                    Quantity = item.Quantity,
                    Image = item.Image,
                    OrderId = newOrder.OrderId
                };
                _db.CartItems.Add(orderDetail);
            }
            newOrder.OrderStatus = "Completed";

            await _db.SaveChangesAsync();

            HttpContext.Session.Remove("Cart");

            TempData["Success"] = "Your order has been placed successfully!";
            return RedirectToAction("OrderConfirmation", new { orderId = newOrder.OrderId });
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

        //      [HttpGet("/orders", Name = "Orders")]
        //public async Task<IActionResult> Orders()
        //{
        //          var user = await _userManager.GetUserAsync(User);
        //          if(user == null) {
        //		return RedirectToAction("Login", "Secure");
        //	}else{
        //		var orders = await _db.Orders.Where(o => o.UserId == user.Id).ToListAsync();
        //		return View(orders);
        //	}


        //}
        [Authorize]
        [HttpGet("/order-detail")]
        public async Task<IActionResult> OrderDetail(int orderId)
        {
            var order = await _db.Orders
                .Include(o => o.CartItems)
                .Include(o => o.PaymentMembers)
                .FirstOrDefaultAsync(o => o.OrderId == orderId);

            if (order == null)
            {
                return Json(new { success = false, message = "Order not found." });
            }

            // Check if the order includes a membership payment
            var membershipPayment = order.PaymentMembers
                .FirstOrDefault(payment => payment.PriceMemberID != 0);

            if (membershipPayment != null)
            {
                // Retrieve Membership and NamePrice details
                var membership = await _db.Memberships
                    .Include(m => m.MemberPriceData) // Include the related MemberPriceData
                    .FirstOrDefaultAsync(m => m.UserID == order.UserId && m.PriceMemberID == membershipPayment.PriceMemberID);

                if (membership == null || membership.MemberPriceData == null)
                {
                    return Json(new { success = false, message = "Membership not active now" });
                }

                // Log details for debugging
                Console.WriteLine($"MembershipName: {membership.MemberPriceData.NamePrice}, Price: {membership.MemberPriceData.Price}");

                // Return the membership details
                return Json(new
                {
                    success = true,
                    isMembership = true,
                    membershipDetails = new
                    {
                        MembershipName = membership.MemberPriceData.NamePrice,
                        Price = membership.MemberPriceData.Price,
                        PurchaseDate = membership.CreateDate.ToString("yyyy-MM-dd"),
                        ExpiryDate = membership.EndDate.ToString("yyyy-MM-dd"),
                        Status = membership.Status ? "Active" : "Expired"
                    }
                });
            }

            // Return cart details if no membership is found
            var cartItems = order.CartItems.Select(item => new
            {
                name = item.Name,
                quantity = item.Quantity,
                price = item.Price
            }).ToList();

            return Json(new
            {
                success = true,
                isMembership = false,
                cartItems
            });
        }



        [HttpGet("/free-recipes", Name = "FreeRecipes")]
        public async Task<IActionResult> FreeRecipes()
        {
            var freeCategory = await _db.Categories.FirstOrDefaultAsync(c => c.Name == "Free" && c.IsActive);
            if (freeCategory == null)
            {
                return View(new List<Recipe>());
            }

            var freeRecipes = await _db.Recipes
                .Include(r => r.Book)
                .Include(r => r.Category)
                .Where(r => r.CategoryId == freeCategory.CategoryId)
                .ToListAsync();

            return View(freeRecipes);
        }

        [HttpGet("/recipe-details/{name}-{id}", Name = "RecipeDetails")]
        public async Task<IActionResult> RecipeDetails(int id)
        {

            var recipe = await _db.Recipes
                .Include(r => r.Category)
                .Include(r => r.Book)
                .Include(r => r.CreatedBy)
                .FirstOrDefaultAsync(r => r.RecipeId == id);

            if (recipe == null)
            {
                return NotFound("Recipe not found.");
            }
            var freeCategory = await _db.Categories.FirstOrDefaultAsync(c => c.Name == "Payment" && c.IsActive);
            if (freeCategory != null)
            {
                ViewBag.CategoryName = "Payment";
                if (freeCategory.CategoryId == recipe.CategoryId)
                {
                    var user = await _userManager.GetUserAsync(User);
                    if (user != null)
                    {
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
                            return View(recipe);

                        }
                        return NotFound("Recipe not found.");


                    }
                    return NotFound("Recipe not found.");


                }
            }
            ViewBag.CategoryName = "Free";

            return View(recipe);
        }
        public async Task<IActionResult> BestSelling()
        {
            
            var books = await _db.Books.ToListAsync(); 
            return View(books);
        }
        [Authorize(Roles = "User")]
        public async Task<IActionResult> SubmitRecipe(RecipeViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                string imagePath = string.Empty;

                // Xử lý upload ảnh
                if (viewModel.ImageUrl != null)
                {
                    string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "ImageUrl");
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

                // Lấy thông tin người dùng hiện tại
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    TempData["Error"] = "Không tìm thấy thông tin người dùng.";
                    return RedirectToAction("Index");
                }

                // Tạo đối tượng Recipe
                var recipe = new Recipe
                {
                    Name = viewModel.Name,
                    Ingredients = viewModel.Ingredients,
                    ImageUrl = imagePath,
                    IsApproved = viewModel.IsApproved,
                    CategoryId = viewModel.CategoryId,
                    BookId = viewModel.BookId,
                    CreatedById = user.Id // Lấy ID của người dùng
                };

                // Lưu công thức vào cơ sở dữ liệu
                _db.Recipes.Add(recipe);
                await _db.SaveChangesAsync();
                TempData["Message"] = "Recipe created successfully!";
                return RedirectToAction(nameof(Index));
            }

            // Nạp lại danh sách Category và Book nếu ModelState không hợp lệ
            viewModel.Categories = _db.Categories.Select(c => new SelectListItem
            {
                Value = c.CategoryId.ToString(),
                Text = c.Name
            }).ToList();

            viewModel.Books = _db.Books.Select(b => new SelectListItem
            {
                Value = b.BookId.ToString(),
                Text = b.Title
            }).ToList();

            return View(viewModel);
        }
    }
}
