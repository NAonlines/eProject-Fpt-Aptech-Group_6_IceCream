using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IceCreamProject.Areas.System.Controllers
{
	[Area("system")]
	[Route("/system/manage")]
	//[Authorize(Roles = "Admin")]
	public class SystemController : Controller
	{
		[Route("index")]
		public IActionResult Index()
		{
			return View();
		}
	}
}
