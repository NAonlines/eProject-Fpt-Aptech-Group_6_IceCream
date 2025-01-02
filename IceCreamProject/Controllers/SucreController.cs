using Microsoft.AspNetCore.Mvc;
using IceCreamProject.Models;
using Microsoft.AspNetCore.Identity;
using IceCreamProject.ViewModels;
namespace IceCreamProject.Controllers
 
{
    

    public class SucreController : Controller
    {
        private readonly ShopContext db;
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly EmailService _emailService;


        public SucreController(ShopContext context, UserManager<User> userManager, SignInManager<User> signInManager, RoleManager<IdentityRole> roleManager, EmailService emailService)
        {
            db = context;
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _emailService = emailService;

        }

        [HttpGet("/login",Name = "Login")]
        public IActionResult Login()
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");

            }
            // This will simply return the login view for GET requests
            return View();
        }

        [HttpPost("/login")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                    var user = await _userManager.FindByEmailAsync(model.Email);
                if (user == null)
                {
                    ModelState.AddModelError("Email", "Email not found.");
                    return View(model);
                }

                //start check and lock
                var result = await _signInManager.PasswordSignInAsync(
                    user, model.Password, model.RememberMe, lockoutOnFailure: true);

                if (result.Succeeded)
                {
                    HttpContext.Session.SetString("UserId", user.Id);
                    HttpContext.Session.SetString("UserName", user.UserName ?? string.Empty);
                    HttpContext.Session.SetString("Email", user.Email ?? string.Empty);
                    return RedirectToAction("Index", "Home");
                }
                else if (result.IsLockedOut)
                {
                    ModelState.AddModelError("", "Your account is locked. Please try again later.");
                    return View(model);
                }
                else if (!await _userManager.CheckPasswordAsync(user, model.Password))
                {
                    ModelState.AddModelError("Password", "Incorrect password.");
                    return View(model);
                }
                else
                {
                    ModelState.AddModelError("", "Login failed. Please try again.");
                    return View(model);
                }
            }

            return View(model);
        }



        [HttpGet("/register",Name = "Register")]
        public IActionResult Register()
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");

            }
            return View();
        }

        [HttpPost("/register")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var existingUser = await _userManager.FindByEmailAsync(model.Email);
                if (existingUser != null)
                {
                    ModelState.AddModelError("Email", "Email already exists");
                    return View(model);
                }
                var user = new User
                {
                    UserName = model.Username,
                    Email = model.Email,
                    PhoneNumber = model.PhoneNumber,
                    Address = model.Address,
                    EmailConfirmed = true,
                };

                var result = await _userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    if (!await _roleManager.RoleExistsAsync("User"))
                    {
                        await _roleManager.CreateAsync(new IdentityRole("User"));
                    }

                    await _userManager.AddToRoleAsync(user, "User");
                    //tự động login
                    await _signInManager.SignInAsync(user, isPersistent: false);
                    TempData["Success"] = "Registration successful!";
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }

            }
            return View(model);
        }
        [HttpGet("/access-denied")]
        public IActionResult AccessDenied()
        {
            return View();
        }
        [HttpGet("/logout",Name = "Logout")]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }



        // Forget password

        [HttpGet("/forgot-password", Name = "ForgotPassword")]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost("/forgot-password")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user == null)
                {
                    ModelState.AddModelError("Email", "Email not found.");
                    return View(model);
                }

                if (user.LastForgotPasswordRequest.HasValue &&
                    user.LastForgotPasswordRequest.Value.AddMinutes(3) > DateTime.UtcNow)
                {
                    ModelState.AddModelError("Email", "You must wait at least 3 minutes before requesting another password reset.");
                    return View(model);
                }

                // Cập nhật thời gian forgetpassword
                user.LastForgotPasswordRequest = DateTime.UtcNow;
                await _userManager.UpdateAsync(user);

                // Generate token and email link
                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                var callbackUrl = Url.Action(
                    "ResetPassword",
                    "Sucre",
                    new { userId = user.Id, token = token },
                    protocol: HttpContext.Request.Scheme);

                await _emailService.SendEmailAsync(user.Email, "Reset Password",
                    $"Click the link to reset your password: {callbackUrl}");

                TempData["Success"] = "Check your email for the password reset link.";
                return RedirectToAction("Login");
            }

            return View(model);
        }


        [HttpGet("/reset-password", Name = "ResetPassword")]
        public IActionResult ResetPassword(string token, string userId)
        {
            if (string.IsNullOrEmpty(token) || string.IsNullOrEmpty(userId))
            {
                ModelState.AddModelError("", "Invalid password reset token.");
                return RedirectToAction("Login");
            }

            var model = new ResetPasswordViewModel { Token = token, UserId = userId };
            return View(model);
        }

        [HttpPost("/reset-password")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var user = await _userManager.FindByIdAsync(model.UserId);
            if (user == null)
            {
                ModelState.AddModelError("", "User not found.");
                return View(model);
            }

            var result = await _userManager.ResetPasswordAsync(user, model.Token, model.NewPassword);
            if (result.Succeeded)
            {
                TempData["Success"] = "Password reset successfully. You can log in now.";
                return RedirectToAction("Login");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }

            return View(model);
        }

    }
}
