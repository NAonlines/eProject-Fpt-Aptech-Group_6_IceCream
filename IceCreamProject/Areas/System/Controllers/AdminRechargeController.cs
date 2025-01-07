using IceCreamProject.Extensions;
using IceCreamProject.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace IceCreamProject.Areas.System.Controllers
{
    

    [Area("System")]
    [Authorize(Roles = "Admin")]
    public class AdminRechargeController : Controller
    {
        private readonly ShopContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public AdminRechargeController(ShopContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        [HttpGet("system/recharge-list")]
        public async Task<IActionResult> Index()
        {
            var recharges = await _context.PaymentMember
                .Include(x => x.UserData)
                .Include(x => x.MemberPriceData)
                .OrderByDescending(x => x.CreateDate)
                .ToListAsync();

            return View(recharges);
        }

        [HttpGet("system/recharge/change-active/{id}")]
        public async Task<IActionResult> ChangeActive(int id)
        {
            try
            {
                var recharge = await _context.PaymentMember.Include(x => x.MemberPriceData).FirstOrDefaultAsync(X => X.IDPayment == id);
                if (recharge == null)
                {
                    return Json(new { code = 400, message = "Deposit not found" });
                }

                recharge.Status = true;
                DateTime endDate = recharge.MemberPriceData.Duration.ToDate();
                var checkOrder = await _context.Memberships.FirstOrDefaultAsync(x => x.UserID == recharge.UserID && x.PriceMemberID == recharge.PriceMemberID);
                if (checkOrder != null)
                {
                    if (checkOrder.Status)
                    {
                        checkOrder.EndDate = endDate;

                    }
                    else
                    {
                        checkOrder.EndDate = endDate;
                        checkOrder.Status = true;

                    }
                }
                else
                {
                    var newOrder = new Memberships
                    {
                        UserID = recharge.UserID,
                        PriceMemberID = recharge.PriceMemberID,
                        CreateDate = DateTime.Now,
                        EndDate = endDate,
                        Status = true
                    };
                    await _context.Memberships.AddAsync(newOrder);
                }

                await _context.SaveChangesAsync();
                return Json(new { code = 200, message = "Success" });

            }
            catch (Exception ex)
            {
                return Json(new { code = 400, message = ex.Message });

            }
        }
    }
}
