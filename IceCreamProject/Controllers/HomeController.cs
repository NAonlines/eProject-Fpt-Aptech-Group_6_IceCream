using Microsoft.AspNetCore.Mvc;

namespace IceCreamProject.Controllers
{
	public class HomeController : Controller
	{
		public IActionResult Index()
		{
			return View();
		}
	}
}
