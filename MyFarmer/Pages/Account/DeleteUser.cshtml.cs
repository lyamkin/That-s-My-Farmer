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
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Globalization;

namespace MyFarmer.Pages.Account
{
    [Authorize(Roles = "User, Farmer")]
    public class DeleteUserModel : PageModel
    {
        private readonly MyFarmerDbContext db;
        private readonly ILogger<DeleteUserModel> logger;
        private readonly UserManager<IdentityUser> userManager;
        private readonly SignInManager<IdentityUser> signInManager;

        private readonly RoleManager<IdentityRole> roleManager;
        public DeleteUserModel(MyFarmerDbContext db, UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager, ILogger<DeleteUserModel> logger, RoleManager<IdentityRole> roleManager)
        {
            this.db = db;
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.logger = logger;
            this.roleManager = roleManager;
        }

        [BindProperty(SupportsGet = true)]
        public string Id { get; set; }

        public IdentityUser UserToDelete;

        public void OnGet()
        {
            UserToDelete = db.Users.FirstOrDefault(i => i.Id == Id);
        }

        public async Task<IActionResult> OnPostDeleteUser()
        {
            var userDelete = userManager.Users.FirstOrDefault(u => u.Id == Id);
            if (userDelete == null)
            {
                // Error Message
                TempData["message"] = $"User account was not found.";
                return RedirectToPage("/Account/UserProfile");
            }

            // delete role if user found
            var roleToDelete = userManager.GetRolesAsync(userDelete).Result.FirstOrDefault();
            await userManager.RemoveFromRoleAsync(userDelete, roleToDelete);
            var result = userManager.DeleteAsync(userDelete);
            // if delete success
            if (result.IsCompletedSuccessfully)
            {
                await signInManager.SignOutAsync();
                // Success message
                TempData["message"] = $"User {userDelete.Email} account is deleted.";
                return RedirectToPage("/Account/Login");
            }
            // Error message
            TempData["message"] = $"User {userDelete.Email} account was not deleted - error encountered.";
            return RedirectToPage("/Account/UserProfile");
        }
    }
}
