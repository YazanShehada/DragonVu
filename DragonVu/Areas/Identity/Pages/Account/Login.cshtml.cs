// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using DragonVu.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;


namespace DragonVu.Areas.Identity.Pages.Account
{
    public class LoginModel : PageModel
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<LoginModel> _logger;

        public LoginModel(
            SignInManager<ApplicationUser> signInManager,
            UserManager<ApplicationUser> userManager,
            ILogger<LoginModel> logger)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _logger = logger;
        }


        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [BindProperty]
        public InputModel Input { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public string ReturnUrl { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [TempData]
        public string ErrorMessage { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public class InputModel
        {
            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Required(ErrorMessage = "الرجاء ادخال رقم الهاتف")]
            //[Phone]
            [Display(Name = "رقم الهاتف")]
            [RegularExpression(@"^\+?\d{8,15}$",
            ErrorMessage = "رقم الهاتف غير صالح")]
            public string UserName { get; set; }

            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Required]
            [DataType(DataType.Password)]
            public string Password { get; set; }

            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Display(Name = "تذكرني ؟")]
            public bool RememberMe { get; set; }
        }

        public async Task OnGetAsync(string returnUrl = null)
        {
            if (!string.IsNullOrEmpty(ErrorMessage))
            {
                ModelState.AddModelError(string.Empty, ErrorMessage);
            }

            returnUrl ??= Url.Content("~/");

            // Clear the existing external cookie to ensure a clean login process
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            ReturnUrl = returnUrl;
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            List<string> countryCodes = new()
                {
                       "+963",
                       "+974",
                       "+966",
                       "+971",
                       "+49",
                       "+968",
                       "+964",
                       "+973",
                       "+90",
                       "+965",
                       "+961",
                       "+20",
                       "+962",
                       "+31",
                       "+249",
                       "+43",
                       "+7",
                       "+46"
                };



            string userInput = Input.UserName;
            IdentityUser user = null;
            Microsoft.AspNetCore.Identity.SignInResult result = null;
            if (userInput.StartsWith("+"))
            {
                user = await _userManager.Users
               .FirstOrDefaultAsync(u => u.UserName == Input.UserName);
                result = await _signInManager.PasswordSignInAsync(
                        Input.UserName,
                        Input.Password,
                        Input.RememberMe,
                        lockoutOnFailure: false);
            }
            else
            {
                string zeroremove = null;
                if (userInput.StartsWith("0"))
                {
                    zeroremove = userInput.TrimStart('0');
                }

                // الحالة 2: بدون رمز → نجرّب كل الدول
                foreach (var code in countryCodes)
                {

                    string fullUser = code + zeroremove;


                    if (fullUser == null)
                    {
                        ModelState.AddModelError(string.Empty, "رقم الهاتف غير مسجل");
                        return Page();
                    }

                    result = await _signInManager.PasswordSignInAsync(
                    fullUser,
                    Input.Password,
                    Input.RememberMe,
                    lockoutOnFailure: false);
                    if (result.Succeeded)
                    {
                        user = await _userManager.Users
                        .FirstOrDefaultAsync(u => u.UserName == fullUser);
                        result = await _signInManager.PasswordSignInAsync(
                        user.UserName,
                        Input.Password,
                        Input.RememberMe,
                        lockoutOnFailure: false);
                        break;
                    }


                }


            }







            returnUrl ??= Url.Action("Index", "Quiz", new { area = "" });

            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            if (ModelState.IsValid)
            {
                // This doesn't count login failures towards account lockout
                // To enable password failures to trigger account lockout, set lockoutOnFailure: true

                if (result.Succeeded)
                {

                    _logger.LogInformation("User logged in.");
                    return LocalRedirect(returnUrl);
                }
                if (result.RequiresTwoFactor)
                {
                    return RedirectToPage("./LoginWith2fa", new { ReturnUrl = returnUrl, RememberMe = Input.RememberMe });
                }
                if (result.IsLockedOut)
                {
                    _logger.LogWarning("User account locked out.");
                    return RedirectToPage("./Lockout");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                    return Page();
                }
            }

            // If we got this far, something failed, redisplay form
            return Page();
        }
    }
}
