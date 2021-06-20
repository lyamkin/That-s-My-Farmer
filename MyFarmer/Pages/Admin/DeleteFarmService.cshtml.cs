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
    public class DeleteFarmServiceModel : PageModel
    {

        private readonly MyFarmerDbContext db;
        private readonly ILogger<UsersModel> logger;
        private readonly UserManager<IdentityUser> userManager;
        private readonly SignInManager<IdentityUser> signInManager;

        private readonly RoleManager<IdentityRole> roleManager;
        public DeleteFarmServiceModel(MyFarmerDbContext db, UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager, ILogger<UsersModel> logger, RoleManager<IdentityRole> roleManager)
        {
            this.db = db;
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.logger = logger;
            this.roleManager = roleManager;
        }

        [BindProperty(SupportsGet = true)]
        public int Id { get; set; }

        [BindProperty(SupportsGet = true)]
        public string Name { get; set; }

        [BindProperty(SupportsGet = true)]
        public string Description { get; set; }
        public void OnPost()
        {
            var farmService = db.FarmsServices.FirstOrDefault(i => i.Id == Id);

        }

        public IActionResult OnPostDeleteFarmService()
        {
            var fsToDelete = db.FarmsServices.FirstOrDefault(u => u.Id == Id);
            if (fsToDelete == null)
            {
                TempData["message"] = $"Farm Service account was not found.";
                return RedirectToPage("/Admin/FarmService");
            }
            db.FarmsServices.Remove(fsToDelete);
            db.SaveChanges();
            TempData["message"] = $"Farm {Name} with Service {Description} was deleted";
            logger.LogInformation($"Farm {Name} with Service {Description} was deleted from database");
            return RedirectToPage("/Admin/FarmService");
        }
        public void OnGet()
        {
        }
    }
}
