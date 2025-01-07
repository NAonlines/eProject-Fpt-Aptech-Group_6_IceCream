using IceCreamProject.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace IceCreamProject.Areas.System.Controllers
{
    

    [Area("System")]
    [Authorize(Roles = "Admin")]
    public class MemberPriceAdminController : Controller
    {
        private readonly ShopContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public MemberPriceAdminController(ShopContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }
        [HttpGet("system/list-price")]
        public async Task<IActionResult> Index()
        {
            var package = await _context.MemberPrice.OrderByDescending(x => x.CreateDate).ToListAsync();
            return View(package);
        }
        [HttpPost("system/add-price")]
        public async Task<IActionResult> AddPackage(MemberPrice model)
        {
            if (model.Price == 0)
            {
                return Json(new { code = 400, message = "The package price must be greater than 0" });
            }
            if (string.IsNullOrEmpty(model.Duration))
            {
                return Json(new { code = 400, message = "Duration cannot be empty" });
            }
            var checkPackage = await _context.MemberPrice.Where(x => x.Duration.Contains(model.Duration)).FirstOrDefaultAsync();
            if (checkPackage != null)
            {
                return Json(new { code = 400, message = "Price type already exists" });

            }
            try
            {
                model.CreateDate = DateTime.Now;
                await _context.AddAsync(model);
                await _context.SaveChangesAsync();
                return Json(new { code = 200, message = "Added successfully" });

            }
            catch (Exception ex)
            {
                return Json(new { code = 400, message = ex.Message });

            }

        }
        [HttpGet("system/get-price/{id}")]
        public async Task<IActionResult> GetPackage(int id)
        {
            var package = await _context.MemberPrice.FirstOrDefaultAsync(x => x.IDMemberShipPrice == id);
            if (package != null)
            {
                return View(package);

            }
            return View(package);


        }
        [HttpPost("system/edit-price")]
        public async Task<IActionResult> EditPackage(MemberPrice model)
        {
            var exitPackage = await _context.MemberPrice.FirstOrDefaultAsync(x => x.IDMemberShipPrice == model.IDMemberShipPrice);
            if (exitPackage == null)
            {
                return Json(new { code = 400, message = "Price member not found" });

            }
            if (model.Price == 0)
            {
                return Json(new { code = 400, message = "Price must be greater than 0" });

            }
            if (string.IsNullOrEmpty(model.Duration))
            {
                return Json(new { code = 400, message = "Duration cannot be empty" });

            }
            var checkPackage = await _context.MemberPrice.Where(x => x.Duration.Contains(model.Duration) && x.IDMemberShipPrice != model.IDMemberShipPrice).FirstOrDefaultAsync();
            if (checkPackage != null)
            {
                return Json(new { code = 400, message = "Package price already exists" });

            }
            try
            {
                exitPackage.Price = model.Price;
                exitPackage.Duration = model.Duration;
                exitPackage.NamePrice = model.NamePrice;
                exitPackage.CreateDate = DateTime.Now;
                await _context.SaveChangesAsync();
                return Json(new { code = 200, message = "Package price repair successful" });

            }
            catch (Exception ex)
            {
                return Json(new { code = 400, message = ex.Message });

            }

        }
        [HttpPost("system/remove-price")]
        public async Task<IActionResult> RemovePackage(int id)
        {

            try
            {
                var package = await _context.MemberPrice.FirstOrDefaultAsync(x => x.IDMemberShipPrice == id);
                if (package == null)
                {
                    return Json(new { code = 400, message = "Package price not found" });

                }

                var listOrder = await _context.PaymentMember.Where(x => x.PriceMemberID == package.IDMemberShipPrice).ToListAsync();
                if (listOrder != null && listOrder.Count > 0)
                {
                    _context.RemoveRange(listOrder);
                }
                _context.Remove(package);
                await _context.SaveChangesAsync();
                return Json(new { code = 200, message = "Package price deletion successful" });

            }
            catch (Exception ex)
            {
                return Json(new { code = 400, message = ex.Message });

            }

        }
    }
}
