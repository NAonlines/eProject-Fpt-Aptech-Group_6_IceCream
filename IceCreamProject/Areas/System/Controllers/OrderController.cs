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

            var order = await db.Orders.FirstOrDefaultAsync(u => u.OrderId == model.OrderId);
            if (order == null)
            {
                return Json(new { success = false, message = "Order not found." });
            }
            order.OrderStatus = model.OrderStatus;
            order.Address = model.Address;
            db.Orders.Update(order);
            await db.SaveChangesAsync();
            return Json(new { success = true, message = "User updated successfully." });
        }
    }




}
