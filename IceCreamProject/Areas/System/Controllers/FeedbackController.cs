using IceCreamProject.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace IceCreamProject.Areas.System.Controllers
{
    [Area("System")]
    [Route("System/[controller]")]
    [Authorize(Roles = "Admin")]
    public class FeedbackController : Controller
    {
        private readonly ShopContext db;

        public FeedbackController(ShopContext context)
        {
            db = context;
        }

        [HttpGet("", Name = "Feedback")]
        public async Task<IActionResult> Index()
        {
            var feedbacks = await db.Feedbacks
                .Include(f => f.User)
                .Select(f => new Feedback
                {
                    FeedbackId = f.FeedbackId,
                    Content = f.Content,
                    SubmittedDate = f.SubmittedDate,
                    Email = f.User != null ? f.User.Email : f.Email, 
                    Name = f.User != null ? f.User.UserName : f.Name,
                    User = f.User 
                })
                .ToListAsync();

            return View(feedbacks);
        }




        // GET: System/Feedback/Delete/5
        [HttpGet("Delete/{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var feedback = await db.Feedbacks.FindAsync(id);

            if (feedback != null)
            {
                db.Feedbacks.Remove(feedback);
                await db.SaveChangesAsync();
                TempData["Message"] = "Feedback deleted successfully.";
            }
            else
            {
                TempData["Message"] = "Feedback not found.";
            }

            // Redirect lại danh sách Feedback
            return RedirectToAction(nameof(Index));
        }

    }
}
