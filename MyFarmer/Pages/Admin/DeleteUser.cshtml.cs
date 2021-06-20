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
    public class DeleteUserModel : PageModel
    {
        private readonly MyFarmerDbContext db;
        private readonly ILogger<UsersModel> logger;
        private readonly UserManager<IdentityUser> userManager;
        private readonly SignInManager<IdentityUser> signInManager;

        private readonly RoleManager<IdentityRole> roleManager;
        public DeleteUserModel(MyFarmerDbContext db, UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager, ILogger<UsersModel> logger, RoleManager<IdentityRole> roleManager)
        {
            this.db = db;
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.logger = logger;
            this.roleManager = roleManager;
        }

        [BindProperty(SupportsGet = true)]
        public string Id { get; set; }

        [BindProperty]
        public string Email { get; set; }

        public void OnPost()
        {
            var user = db.Users.FirstOrDefault(i => i.Id == Id);
            Email = user.Email;
        }

        public async Task<IActionResult> OnPostDeleteUser()
        {
            var userToDelete = userManager.Users.FirstOrDefault(u => u.Id == Id);
            if (userToDelete == null)
            {
                TempData["message"] = $"User account was not found.";
                return RedirectToPage("/Admin/Users");
            }

            var roleToDelete = userManager.GetRolesAsync(userToDelete).Result.FirstOrDefault();
            if (roleToDelete == "Farmer")
            {
                var farms = db.Farms.Where(f => f.Farmer.Id == Id).ToList();
                if (farms != null)
                {
                    foreach (var f in farms)
                    {
                        var farmFoods = db.FarmsFoods.Where(ff => ff.Farm.Id == f.Id).ToList();
                        if (farmFoods != null)
                        {
                            db.FarmsFoods.RemoveRange(farmFoods);
                        }
                        var farmServices = db.FarmsServices.Where(fs => fs.Farm.Id == f.Id).ToList();
                        if (farmServices != null)
                        {
                            db.FarmsServices.RemoveRange(farmServices);
                        }
                        db.Farms.Remove(f);
                    }
                }
            }
            db.SaveChanges();
            var result1 = await userManager.RemoveFromRoleAsync(userToDelete, roleToDelete);
            if (result1.Succeeded)
            {
                var result = userManager.DeleteAsync(userToDelete);
                if (result.IsCompletedSuccessfully)
                {
                    TempData["message"] = $"User {userToDelete.Email} account is deleted.";
                    return RedirectToPage("/Admin/Users");
                }
            }
            TempData["message"] = $"User {userToDelete.Email} account was not deleted.";
            return RedirectToPage("/Admin/Users");
        }
    }


}

