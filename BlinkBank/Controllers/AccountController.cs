using BlinkBank.Models;
using BlinkBank.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

namespace BlinkBank.Controllers
{
    public class AccountController : Controller
    {
        private readonly SignInManager<ApplicationUser> _signeInManager;//for login
        private readonly UserManager<ApplicationUser> _userManager;//for register
        private readonly BankDBContext _context;

        public AccountController(SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager, BankDBContext context)
        {
            this._signeInManager = signInManager;
            this._userManager = userManager;
            this._context = context;

        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterVM model)
        {
            if (ModelState.IsValid)
            {
                // إضافة مستخدم جديد
                ApplicationUser user = new()
                {
                    Name = model.Name,
                    Email = model.Email,
                    UserName = model.Email
                };

                var result = await _userManager.CreateAsync(user, model.Password!);
                if (result.Succeeded)
                {
                    await _signeInManager.SignInAsync(user, true);
                    var account = await _context.Accounts.FirstOrDefaultAsync(a => a.ApplicationUserId == user.Id);

                    if (account != null)
                    {
                        return RedirectToAction("Index", "Transactions");
                    }
                    else
                    {
                        // تمرير البيانات باستخدام TempData
                        TempData["RegisteredName"] = model.Name;
                        TempData["RegisteredEmail"] = model.Email;

                        return RedirectToAction("CreateAccount", "AccountManagement");
                    }
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                    Console.WriteLine($"Error: {error.Description}");
                }
            }

            return View(model);
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginVM model)
        {
            if (ModelState.IsValid)
            {
                //login
                var result = await _signeInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, false);
                if (result.Succeeded)
                {
                    var user = await _userManager.FindByEmailAsync(model.Email);
                    var account = await _context.Accounts.FirstOrDefaultAsync(a => a.ApplicationUserId == user.Id);
                    if (account != null)
                    {
                        return RedirectToAction("Index", "Transactions");
                    }
                    else
                    {
                        return RedirectToAction("CreateAccount", "AccountManagement");
                    }

                }
                ModelState.AddModelError(string.Empty, "Invalid login attempt.");

            }
            return View(model);
        }
        public async Task<IActionResult> Logout()
        {
            await _signeInManager.SignOutAsync();
            return RedirectToAction("index", "Home");
        }
    }
}
