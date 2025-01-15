using IceCreamProject.Models;
using IceCreamProject.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace IceCreamProject.Areas.System.Controllers
{
    [Area("System")]
    [Route("System/[controller]")]
    [Authorize(Roles = "Admin")]
    public class OrderController : Controller
	{
		private readonly ShopContext db;
		public OrderController(ShopContext context) {
			db = context;
        }
        [HttpGet("", Name = "Order")]
		public  async Task<IActionResult> Index()
		{
			return View(await db.Orders.ToListAsync());
		}

        [HttpPost("Delete")]
        public async Task<IActionResult> Delete(int orderId)
        {
            var order = await db.Orders.FirstOrDefaultAsync(u => u.OrderId == orderId);
            if (order == null)
            {
                return Json(new { success = false, message = "Order not found." });
            }

            db.Orders.Remove(order);
            await db.SaveChangesAsync();

            return Json(new { success = true, message = "Order deleted successfully." });
        }



        [HttpGet("Edit/{id}")]
        public async Task<IActionResult> Edit(int id)
        {
            var order = await db.Orders.FirstOrDefaultAsync(u => u.OrderId == id);
            if (order == null)
            {
                return NotFound();
            }

            var model = new OrderViewModel
            {
                OrderId = order.OrderId,
                CustomerName = order.CustomerName,
                UserId = order.UserId,
                OrderDate = order.OrderDate,
                Address = order.Address,
                PaymentMethod = order.PaymentMethod,
                OrderStatus = order.OrderStatus,
                TotalAmount = order.TotalAmount
            };

            return View(model);
        }


        [HttpPost("Edit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(OrderViewModel model)
        {
            var allowedTransitions = new Dictionary<string, List<string>>()
    {
        { "Processing", new List<string> { "Shipped", "Cancelled" } },
        { "Shipped", new List<string> { "Delivered", "Cancelled" } },
        { "Delivered", new List<string>() },
        { "Cancelled", new List<string>() }
    };
            var order = await db.Orders.FirstOrDefaultAsync(u => u.OrderId == model.OrderId);
            if (order == null)
            {
                return Json(new { success = false, message = "Order not found." });
            }

            if (!allowedTransitions.ContainsKey(order.OrderStatus) || !allowedTransitions[order.OrderStatus].Contains(model.OrderStatus))
            {
                return Json(new { success = false, message = $"Invalid status transition from '{order.OrderStatus}' to '{model.OrderStatus}'." });
            }
            if (order.OrderStatus == "Cancelled")
            {
                return Json(new { success = false, message = "Cancelled orders cannot be updated." });
            }
            //if (order.OrderStatus == "Completed" && model.OrderStatus == "Processing")
            //{
            //    return Json(new { success = false, message = "Cannot change status from Shipped to Processing." });
            //}

            //if (order.OrderStatus == "Cancelled")
            //{
            //    return Json(new { success = false, message = "Cancelled orders cannot be updated." });
            //}
            order.OrderStatus = model.OrderStatus;
            order.Address = model.Address;
            db.Orders.Update(order);
            await db.SaveChangesAsync();
            return Json(new { success = true, message = "User updated successfully." });
        }

        [HttpGet("Details/{id}")]
        public async Task<IActionResult> Details(int id)
        {
            var cartItems = db.CartItems.Where(u => u.OrderId == id).ToList();
            if (!cartItems.Any())
            {
                return NotFound();
            }

            return View(cartItems);
        }

    }




}
