using Microsoft.AspNetCore.Mvc;

namespace IceCreamProject.Controllers
{
	public class HomeController : Controller
	{
		[HttpGet("/",Name = "Home")]
		public IActionResult Index()
		{
			return View();
		}
		[HttpGet("/about-us",Name = "AboutUs")]
		public IActionResult AboutUs()
		{
			return View();
		}
		[HttpGet("/contact-us",Name = "ContactUs")]
		public IActionResult ContactUs()
		{
			return View();
		}
	}
}
