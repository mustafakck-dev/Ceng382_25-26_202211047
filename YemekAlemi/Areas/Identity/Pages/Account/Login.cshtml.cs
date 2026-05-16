// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using YemekAlemi.Data;
using YemekAlemi.Models;
using YemekAlemi.Services;

namespace YemekAlemi.Areas.Identity.Pages.Account
{
    public class LoginModel : PageModel
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly ILogger<LoginModel> _logger;
        private readonly AppDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly EmailService _emailService;

        public LoginModel(
            SignInManager<IdentityUser> signInManager,
            ILogger<LoginModel> logger,
            UserManager<IdentityUser> userManager,
            AppDbContext context,
            EmailService emailService)
        {
            _signInManager = signInManager;
            _logger = logger;
            _userManager = userManager;
            _context = context;
            _emailService = emailService;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        public string ReturnUrl { get; set; }

        [TempData]
        public string ErrorMessage { get; set; }

        public class InputModel
        {
            [Required]
            [EmailAddress]
            public string Email { get; set; }

            [Required]
            [DataType(DataType.Password)]
            public string Password { get; set; }

            [Display(Name = "Remember me?")]
            public bool RememberMe { get; set; }
        }

        public async Task OnGetAsync(string returnUrl = null)
        {
            if (!string.IsNullOrEmpty(ErrorMessage))
            {
                ModelState.AddModelError(string.Empty, ErrorMessage);
            }

            returnUrl ??= Url.Content("~/");

            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

            ExternalLogins =
                (await _signInManager
                    .GetExternalAuthenticationSchemesAsync())
                .ToList();

            ReturnUrl = returnUrl;
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");

            ExternalLogins =
                (await _signInManager
                    .GetExternalAuthenticationSchemesAsync())
                .ToList();

            if (ModelState.IsValid)
            {
                var user =
                    await _userManager.FindByEmailAsync(Input.Email);

                if (user != null)
                {
                    var passwordValid =
                        await _userManager.CheckPasswordAsync(
                            user,
                            Input.Password);

                    if (passwordValid)
                    {
                        var code =
                            new Random()
                                .Next(100000, 999999)
                                .ToString();

                        HttpContext.Session.SetString(
                            "TwoFactorUserId",
                            user.Id);

                        HttpContext.Session.SetString(
                            "TwoFactorCode",
                            code);

                        HttpContext.Session.SetString(
                            "TwoFactorRememberMe",
                            Input.RememberMe.ToString());

                        _emailService.SendEmail(
                            Input.Email,
                            "YemekAlemi Two-Factor Login Code",
                            $"Your YemekAlemi verification code is: {code}",
                            user.Id
                        );

                        _context.AppLogs.Add(new AppLog
                        {
                            EventType = "Login",
                            Message = $"Successful login for: {Input.Email}",
                            UserEmail = Input.Email,
                            CreatedAt = DateTime.Now
                        });

                        _context.AppLogs.Add(new AppLog
                        {
                            EventType = "Security",
                            Message = $"2FA code generated for: {Input.Email}",
                            UserEmail = Input.Email,
                            CreatedAt = DateTime.Now
                        });

                        _context.SaveChanges();

                        return RedirectToPage("./VerifyTwoFactor");
                    }
                }

                _context.AppLogs.Add(new AppLog
                {
                    EventType = "Login",
                    Message = $"Failed login attempt: {Input.Email}",
                    UserEmail = Input.Email,
                    CreatedAt = DateTime.Now
                });

                _context.SaveChanges();

                ModelState.AddModelError(
                    string.Empty,
                    "Invalid login attempt.");

                return Page();
            }

            return Page();
        }
    }
}