using Microsoft.AspNetCore.Mvc;
using IceCreamProject.Models;
using Microsoft.AspNetCore.Identity;
namespace IceCreamProject.Controllers
{
    

    public class SucreController : Controller
    {
        private readonly ShopContext db;
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;


        public SucreController(ShopContext context, UserManager<User> userManager, SignInManager<User> signInManager, RoleManager<IdentityRole> roleManager)
        {
            db = context;
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
        }

        [HttpGet("/login")]
        public IActionResult Login()
        {
            return View();
        }
        [HttpGet("/register")]
        public IActionResult Register()
        {
            return View();
        }
    }
}
