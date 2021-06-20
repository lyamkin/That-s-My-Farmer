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

namespace MyFarmer.Pages.Admin
{
    [Authorize(Roles = "Admin")]
    public class DeleteFoodModel : PageModel
    {
        private readonly MyFarmerDbContext db;
        private readonly ILogger<UsersModel> logger;
        private readonly UserManager<IdentityUser> userManager;
        private readonly SignInManager<IdentityUser> signInManager;

        private readonly RoleManager<IdentityRole> roleManager;
        public DeleteFoodModel(MyFarmerDbContext db, UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager, ILogger<UsersModel> logger, RoleManager<IdentityRole> roleManager)
        {
            this.db = db;
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.logger = logger;
            this.roleManager = roleManager;
        }

        [BindProperty(SupportsGet = true)]
        public int Id { get; set; }

        [BindProperty]
        public string Description { get; set; }

        public void OnPost()
        {
            var food = db.Foods.FirstOrDefault(i => i.Id == Id);
            Description = food.Description;
        }

        public IActionResult OnPostDeleteFood()
        {
            var foodToDelete = db.Foods.FirstOrDefault(u => u.Id == Id);
            if (foodToDelete == null)
            {
                TempData["message"] = $"Food account was not found.";
                return RedirectToPage("/Admin/Food");
            }
            db.Foods.Remove(foodToDelete);
            db.SaveChanges();
            TempData["message"] = $"Food {foodToDelete.Description} is deleted.";
            return RedirectToPage("/Admin/Food");
        }
        public void OnGet()
        {
        }
    }
}
