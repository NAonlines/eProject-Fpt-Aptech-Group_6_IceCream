using IceCreamProject.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace IceCreamProject.Controllers
{
   
    public class ReadReplyFeedbackController : Controller
	{
		private readonly ShopContext _db;
		private readonly UserManager<User> _userManager;
		private readonly IWebHostEnvironment _webHostEnvironment;
		public ReadReplyFeedbackController(ShopContext db, UserManager<User> userManager, IWebHostEnvironment webHostEnvironment)
		{
			_db = db;
			_userManager = userManager;
			_webHostEnvironment = webHostEnvironment;
		}

		[HttpGet("read-reply-feedback", Name = "ReadReplyFeedback")]
		public async Task<IActionResult> Index()
		{
			var user = await _userManager.GetUserAsync(User);
			if (user == null)
			{
				return NotFound();
			}

			var feedbacks = await _db.Feedbacks
				.Where(f => f.UserId == user.Id) 
				.ToListAsync();

            var feedbackIds = feedbacks.Select(f => f.FeedbackId).ToList();

            var feedbackResponses = await _db.FeedbackResponses
			.Where(fr => feedbackIds.Contains(fr.FeedbackId))
			.ToListAsync();

            var viewModel = new ReadReplyFeedbackViewModel
			{
				Feedbacks = feedbacks,
				FeedbackResponses = feedbackResponses
			};

			return View(viewModel); 
		}


	}
}
