using IceCreamProject.Models;
using IceCreamProject.Services;
using IceCreamProject.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace IceCreamProject.Controllers
{
    
    public class PaymentController : Controller
    {
        private readonly ShopContext _db;
        private readonly UserManager<User> _userManager;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IServices _iService;

        public PaymentController(ShopContext db, UserManager<User> userManager, IWebHostEnvironment webHostEnvironment, IServices iService)
        {
            _db = db;
            _userManager = userManager;
            _webHostEnvironment = webHostEnvironment;
            _iService = iService;
        }

        [HttpGet("payment/{id}/{price}")]
        public async Task<IActionResult> Index(int id, decimal price)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Redirect("/login");
            }
            ViewBag.CodeOrder = _iService.GenerateRandomString(6);
            ViewBag.PackageID = id;
            ViewBag.Price = price;
            return View();
        }
        [HttpPost("confirm/payment")]
        public async Task<IActionResult> ConfirmRecharge(PaymentViewModel model)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Redirect("/login");
            }
            try
            {

                var napTien = new PaymentMember
                {
                    UserID = user.Id,
                    PriceMemberID = model.PriceMemberID,
                    PaymentCode = model.OrderCode,
                    Price = model.PriceRecharge,
                    Status = false,
                    CreateDate = DateTime.Now,
                };
                await _db.AddAsync(napTien);
                await _db.SaveChangesAsync();
                return Json(new { code = 200, message = "Payment confirmed" });

            }
            catch (Exception ex)
            {
                return Json(new { code = 400, message = ex.Message });

            }



        }
    }
}
