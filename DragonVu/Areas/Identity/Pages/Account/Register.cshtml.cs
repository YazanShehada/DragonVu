// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using DragonVu.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;


namespace DragonVu.Areas.Identity.Pages.Account
{


    public class RegisterModel : PageModel
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUserStore<ApplicationUser> _userStore;
        private readonly IUserEmailStore<ApplicationUser> _emailStore;
        private readonly ILogger<RegisterModel> _logger;
        private readonly IEmailSender _emailSender;
        public List<SelectListItem> CountryCodes { get; set; }


        public RegisterModel(

            UserManager<ApplicationUser> userManager,
            IUserStore<ApplicationUser> userStore,
            SignInManager<ApplicationUser> signInManager,
            ILogger<RegisterModel> logger,
            IEmailSender emailSender)
        {
            _userManager = userManager;
            _userStore = userStore;

            _signInManager = signInManager;
            _logger = logger;
            _emailSender = emailSender;

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
        public string ReturnUrl { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public class InputModel
        {


            //[Required(ErrorMessage = "الرجاء ادخال اسم المستخدم")]
            //[Display(Name = "اسم المستخدم")]


            [Required(ErrorMessage = "الرجاء ادخال رقم الهاتف")]
            //[Phone]
            [Display(Name = "رقم الهاتف")]
            [RegularExpression(@"^\+\d{8,15}$",
            ErrorMessage = "رقم الهاتف غير صالح")]



            public string UserName { get; set; }
            [Required(ErrorMessage = "الرجاء ادخال اسم المستخدم")]
            [Display(Name = "اسم المستخدم")]
            public string Name { get; set; }

            //[Required(ErrorMessage = "الرجاء ادخال رقم الهاتف")]
            ////[Phone]
            //[Display(Name = "رقم الهاتف")]
            //[RegularExpression(@"^\+\d{8,15}$",
            //ErrorMessage = "رقم الهاتف غير صالح")]
            //public string PhoneNumber { get; set; }


            [Required(ErrorMessage = "الرجاء ادخال كلمة المرور")]
            [StringLength(100, MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "كلمة المرور")]
            public string Password { get; set; }


        }
        private void LoadCountryCodes()
        {
            CountryCodes = new List<SelectListItem>
            {


    new SelectListItem { Text = "🇸🇾 (+963)", Value = "+963" },
    new SelectListItem { Text = "🇶🇦 (+974)", Value = "+974" },
    new SelectListItem { Text = "🇸🇦 (+966)", Value = "+966" },
    new SelectListItem { Text = "🇦🇪 (+971)", Value = "+971" },
    new SelectListItem { Text = "🇩🇪 (+49)", Value = "+49" },
    new SelectListItem { Text = "🇴🇲 (+968)", Value = "+968" },
    new SelectListItem { Text = "🇮🇶 (+964)", Value = "+964" },
    new SelectListItem { Text = "🇧🇭 (+973)", Value = "+973" },
    new SelectListItem { Text = "🇹🇷 (+90)", Value = "+90" },
    new SelectListItem { Text = "🇰🇼 (+965)", Value = "+965" },
    new SelectListItem { Text = "🇱🇧 (+961)", Value = "+961" },
    new SelectListItem { Text = "🇪🇬 (+20)", Value = "+20" },
    new SelectListItem { Text = "🇯🇴 (+962)", Value = "+962" },
    new SelectListItem { Text = "🇳🇱 (+31)", Value = "+31" },
    new SelectListItem { Text = "🇸🇩 (+249)", Value = "+249" },
    new SelectListItem { Text = "🇦🇹 (+43)", Value = "+43" },
    new SelectListItem { Text = "🇷🇺 (+7)", Value = "+7" },
    new SelectListItem { Text = "🇸🇪 (+46)", Value = "+46" }
};

        }




        public void OnGet(string returnUrl = null)
        {
            ReturnUrl = returnUrl;
            LoadCountryCodes();
        }


        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {

            returnUrl ??= returnUrl ??= Url.Action("Index", "Quiz", new { area = "" });
            LoadCountryCodes();


            if (!ModelState.IsValid)
                return Page();

            var user = new ApplicationUser
            {
                UserName = Input.UserName,
                //PhoneNumber = Input.PhoneNumber,


                Name = Input.Name,
                IsActive = false,
                CreatedAt = DateTime.UtcNow
            };

            var result = await _userManager.CreateAsync(user, Input.Password);

            if (result.Succeeded)
            {
                _logger.LogInformation("User created.");
                await _userManager.AddToRoleAsync(user, "User");

                // تسجيل دخول مباشر بدون أي تأكيد
                await _signInManager.SignInAsync(user, isPersistent: false);
                return LocalRedirect(returnUrl);
            }

            foreach (var error in result.Errors)
            {

                if (error.Code == "DuplicateUserName")
                {
                    ModelState.AddModelError(string.Empty, "رقم الهاتف هذا مستخدم مسبقاً، الرجاء اختيار رقم آخر.");
                }
                else
                    ModelState.AddModelError(string.Empty, error.Description);
            }

            return Page();
        }


        private ApplicationUser CreateUser()
        {
            try
            {
                return Activator.CreateInstance<ApplicationUser>();
            }
            catch
            {
                throw new InvalidOperationException($"Can't create an instance of '{nameof(ApplicationUser)}'. " +
                    $"Ensure that '{nameof(ApplicationUser)}' is not an abstract class and has a parameterless constructor, or alternatively " +
                    $"override the register page in /Areas/Identity/Pages/Account/Register.cshtml");
            }
        }


    }
}
