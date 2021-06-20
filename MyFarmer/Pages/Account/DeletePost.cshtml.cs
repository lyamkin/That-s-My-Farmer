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
    [Authorize(Roles = "User")]
    public class DeletePostModel : PageModel
    {
        private readonly MyFarmerDbContext db;
        private readonly ILogger<DeleteUserModel> logger;
        private readonly UserManager<IdentityUser> userManager;
        private readonly SignInManager<IdentityUser> signInManager;

        private readonly RoleManager<IdentityRole> roleManager;
        public DeletePostModel(MyFarmerDbContext db, UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager, ILogger<DeleteUserModel> logger, RoleManager<IdentityRole> roleManager)
        {
            this.db = db;
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.logger = logger;
            this.roleManager = roleManager;
        }

        [BindProperty(SupportsGet = true)]
        public int Id { get; set; }

        public CustomerComment PostToDelete;

        public void OnGet()
        {
            PostToDelete = db.CustomerComments.FirstOrDefault(c => c.Id == Id);
        }

        public async Task<IActionResult> OnPostDeletePost()
        {
            var postDelete = db.CustomerComments.FirstOrDefault(p => p.Id == Id);
            if (postDelete == null)
            {
                // Error Message
                TempData["message"] = $"Post was not found.";
                return RedirectToPage("/Account/UserProfile");
            }

            // delete post if post found
            db.CustomerComments.Remove(postDelete);
            db.SaveChanges();
            TempData["message"] = $"Post deleted successfully.";
            return RedirectToPage("/Account/UserProfile");
        }
    }
}
