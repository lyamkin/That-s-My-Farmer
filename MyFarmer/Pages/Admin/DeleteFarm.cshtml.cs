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
    public class DeleteFarmModel : PageModel
    {
        private readonly MyFarmerDbContext db;
        private readonly ILogger<UsersModel> logger;
        private readonly UserManager<IdentityUser> userManager;
        private readonly SignInManager<IdentityUser> signInManager;

        private readonly RoleManager<IdentityRole> roleManager;
        public DeleteFarmModel(MyFarmerDbContext db, UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager, ILogger<UsersModel> logger, RoleManager<IdentityRole> roleManager)
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
        public string Name { get; set; }

        public void OnPost()
        {
            var farm = db.Farms.FirstOrDefault(i => i.Id == Id);
            Name = farm.Name;
        }

        public IActionResult OnPostDeleteFarm()
        {
            var farmToDelete = db.Farms.FirstOrDefault(u => u.Id == Id);
            if (farmToDelete == null)
            {
                TempData["message"] = $"Farm account was not found.";
                return RedirectToPage("/Admin/Farms");
            }
            var ff = db.FarmsFoods.Where(f => f.Farm.Id == Id).ToList();
            if (ff != null)
            {
                db.FarmsFoods.RemoveRange(ff);
            }
            var fs = db.FarmsServices.Where(f => f.Farm.Id == Id).ToList();
            if (ff != null)
            {
                db.FarmsServices.RemoveRange(fs);
            }
            db.Farms.Remove(farmToDelete);
            db.SaveChanges();
            TempData["message"] = $"User {Name} account is deleted.";
            return RedirectToPage("/Admin/Farms");
        }
    }
}
