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
	}
}
