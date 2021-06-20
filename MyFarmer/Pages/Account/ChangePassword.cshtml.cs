using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using MyFarmer.Data;
using MyFarmer.Models;
using Microsoft.AspNetCore.Authorization;

namespace MyFarmer.Pages.Account
{
    [Authorize(Roles = "User, Farmer")]
    public class ChangePasswordModel : PageModel
    {
        private readonly MyFarmerDbContext db;
        private readonly ILogger<ChangePasswordModel> logger;
        private readonly UserManager<IdentityUser> userManager;
        private readonly SignInManager<IdentityUser> signInManager;

        private readonly RoleManager<IdentityRole> roleManager;

        [TempData]
        public string Message { get; set; }

        [BindProperty(SupportsGet = true)]
        public string Id { get; set; }

        public ChangePasswordModel(MyFarmerDbContext db, UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager, ILogger<ChangePasswordModel> logger, RoleManager<IdentityRole> roleManager)
        {
            this.db = db;
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.logger = logger;
            this.roleManager = roleManager;
        }
        public IdentityUser currUser;

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            // public string UserId { get; set; }

            [Required]
            [DataType(DataType.Password, ErrorMessage = "Current password required")]
            [Display(Name = "Current Password:")]
            public string CurrentPassword { get; set; }

            [Required]
            [Display(Name = "New Password:")]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and max {1} characters long.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            public string NewPassword { get; set; }

            [Required]
            [DataType(DataType.Password)]
            [Display(Name = "Confirm Password:")]
            [Compare("NewPassword", ErrorMessage = "New password and confirmation password do not match.")]
            public string NewPasswordConfirm { get; set; }

        }
        public void OnGet()
        {
            currUser = userManager.Users.FirstOrDefault(i => i.UserName == @User.Identity.Name);
            if (currUser == null)
            {
                RedirectToPage("/Account/Login");
            }
        }
        public async Task<IActionResult> OnPost()
        {
            if (ModelState.IsValid)
            {
                // get user
                var user = userManager.Users.FirstOrDefault(i => i.Id == Id);

                // if null, redirect
                if (user == null)
                {
                    return RedirectToPage("/Account/UserProfile");
                }

                // update password
                var result = await userManager.ChangePasswordAsync(user, Input.CurrentPassword, Input.NewPassword);

                // if result does not succeed, add errors to the model state and re-render the ChangePassword view
                if (!result.Succeeded)
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                    currUser = userManager.Users.FirstOrDefault(i => i.Id == Id);

                    return Page();
                }

                //  refresh sign in cookie and re-direct user to UserProfile page
                await signInManager.RefreshSignInAsync(user);
                Message = $"Password updated successfully.";
                return RedirectToPage("/Account/UserProfile");
            }

            // if modelState is not valid, re-render page
            currUser = userManager.Users.FirstOrDefault(i => i.Id == Id);
            return Page();
        }


    }
}
