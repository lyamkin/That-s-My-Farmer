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

namespace MyFarmer.Account.Pages
{
    [Authorize(Roles = "User, Farmer")]
    public class UserProfileModel : PageModel
    {
        private readonly MyFarmerDbContext db;
        private readonly ILogger<UserProfileModel> logger;
        private readonly UserManager<IdentityUser> userManager;
        private readonly SignInManager<IdentityUser> signInManager;

        private readonly RoleManager<IdentityRole> roleManager;

        [TempData]
        public string Message { get; set; }

        public string Id { get; set; }

        public UserProfileModel(MyFarmerDbContext db, UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager, ILogger<UserProfileModel> logger, RoleManager<IdentityRole> roleManager)
        {
            this.db = db;
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.logger = logger;
            this.roleManager = roleManager;
        }
        public IdentityUser currUser;
        public List<CustomerComment> custCommentsList = new List<CustomerComment>();

        public void OnGet()
        {
            currUser = userManager.Users.FirstOrDefault(i => i.UserName == @User.Identity.Name);
            custCommentsList = db.CustomerComments.Include(f => f.Farm).Where(c => c.Customer.Id == currUser.Id).OrderByDescending(c => c.Created).ToList();
            // FarmList = db.Farms.Include(m => m.Farmer).ToList();
            if (currUser == null)
            {
                RedirectToPage("/Account/Login");
            }
        }
    }
}
