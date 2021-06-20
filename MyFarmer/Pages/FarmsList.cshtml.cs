using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MyFarmer.Data;
using MyFarmer.Models;

namespace MyFriends2.Pages
{
    [Authorize(Roles = "Farmer")]
    public class FarmsListModel : PageModel
    {
        private readonly MyFarmerDbContext db;
        private readonly ILogger<AddFarmModel> logger;
        private readonly SignInManager<IdentityUser> signInManager;
        private readonly UserManager<IdentityUser> userManager;

        public FarmsListModel(MyFarmerDbContext db, ILogger<AddFarmModel> logger,SignInManager<IdentityUser> signInManager, UserManager<IdentityUser> userManager)
        {
            this.db = db;
            this.logger = logger;
            this.signInManager = signInManager;
            this.userManager = userManager;
        }
        [BindProperty]
        public List<Farm> FarmList {get;set;}
        [BindProperty]
        public IdentityUser Farmer { get; set; }
        [TempData]
        public string Message { get; set; }
        public async Task<IActionResult> OnGetAsync()
        {
            // Get the user
            var user = await userManager.FindByNameAsync(User.Identity.Name);
            Farmer = user;
            //get the list of the farm for defined user
            FarmList = db.Farms.Include(farm => farm.Farmer).Where(farm => farm.Farmer.Id == user.Id).ToList();

            return Page();
        }
    }
}
