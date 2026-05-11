using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using YemekAlemi.Data;
using YemekAlemi.Models;

namespace YemekAlemi.Areas.Identity.Pages.Account
{
    public class VerifyTwoFactorModel : PageModel
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly AppDbContext _context;

        public VerifyTwoFactorModel(
            SignInManager<IdentityUser> signInManager,
            UserManager<IdentityUser> userManager,
            AppDbContext context)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _context = context;
        }

        [BindProperty]
        public string Code { get; set; } = string.Empty;

        public string? ErrorMessage { get; set; }

        public IActionResult OnGet()
        {
            var userId = HttpContext.Session.GetString("TwoFactorUserId");

            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToPage("./Login");
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var userId = HttpContext.Session.GetString("TwoFactorUserId");
            var savedCode = HttpContext.Session.GetString("TwoFactorCode");
            var rememberMeText = HttpContext.Session.GetString("TwoFactorRememberMe");

            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(savedCode))
            {
                return RedirectToPage("./Login");
            }

            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
            {
                return RedirectToPage("./Login");
            }

            if (Code == savedCode)
            {
                bool rememberMe = rememberMeText == "True";

                await _signInManager.SignInAsync(user, rememberMe);

                _context.AppLogs.Add(new AppLog
                {
                    EventType = "Security",
                    Message = $"2FA verification successful: {user.Email}",
                    UserEmail = user.Email,
                    CreatedAt = DateTime.Now
                });

                _context.SaveChanges();

                HttpContext.Session.Remove("TwoFactorUserId");
                HttpContext.Session.Remove("TwoFactorCode");
                HttpContext.Session.Remove("TwoFactorRememberMe");

                return RedirectToAction("Index", "Dashboard");
            }

            _context.AppLogs.Add(new AppLog
            {
                EventType = "Security",
                Message = $"Failed 2FA attempt: {user.Email}",
                UserEmail = user.Email,
                CreatedAt = DateTime.Now
            });

            _context.SaveChanges();

            ErrorMessage = "Invalid verification code.";
            return Page();
        }
    }
}