using Microsoft.AspNetCore.Mvc;
using IceCreamProject.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace IceCreamProject.Areas.System.Controllers
{
    [Area("system")]
    [Route("/system/[controller]")]
    [Authorize(Roles = "Admin")]
    public class DashboardController : Controller
    {
        private readonly ShopContext _context;
        private readonly UserManager<User> _userManager;

        public DashboardController(ShopContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [Route("",Name = "DashBoard")]
        public async Task<IActionResult> Index()
        {
            var totalOrders = await _context.Orders.CountAsync();
            var totalAmount = await _context.Orders.SumAsync(o => o.TotalAmount);
            var totalCartItems = await _context.CartItems.CountAsync();
            var mostRecentOrders = await _context.Orders
                .OrderByDescending(o => o.OrderDate)
                .Take(5)
                .ToListAsync();
            var totalUsers = await _userManager.Users.CountAsync();

            // Orders by day
            var ordersByDay = await _context.Orders
                .GroupBy(o => o.OrderDate.Date)
                .Select(g => new { Date = g.Key, Count = g.Count() })
                .ToListAsync();

            ViewData["TotalOrders"] = totalOrders;
            ViewData["TotalAmount"] = totalAmount;
            ViewData["TotalCartItems"] = totalCartItems;
            ViewData["MostRecentOrders"] = mostRecentOrders;
            ViewData["TotalUsers"] = totalUsers;

            ViewData["OrderDates"] = ordersByDay.Select(o => o.Date.ToString("MM/dd/yyyy"));
            ViewData["OrdersCount"] = ordersByDay.Select(o => o.Count);

            return View();
        }

    }
}
