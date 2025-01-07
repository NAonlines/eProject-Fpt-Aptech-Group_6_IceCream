using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace IceCreamProject.Areas.System.Controllers
{
   

    [Area("System")]
    [Authorize(Roles = "Admin")]
    public class MembershipAdminController : Controller
    {
        private readonly ShopContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public MembershipAdminController(ShopContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        [HttpGet("system/membership-list")]
        public async Task<IActionResult> Index()
        {
            var memberships = await _context.Memberships
                .Include(x => x.UserData)
                .Include(x => x.MemberPriceData)
                .OrderByDescending(x => x.CreateDate)
                .ToListAsync();

            return View(memberships);
        }
    }
}
