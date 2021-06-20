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

namespace MyFarmer.Pages
{
    public class IndexModel : PageModel
    {
        private readonly MyFarmerDbContext db;
        private readonly ILogger<IndexModel> logger;
        private readonly UserManager<IdentityUser> userManager;
        private readonly SignInManager<IdentityUser> signInManager;

        private readonly RoleManager<IdentityRole> roleManager;

        public IndexModel(MyFarmerDbContext db, UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager, ILogger<IndexModel> logger, RoleManager<IdentityRole> roleManager)
        {
            this.db = db;
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.logger = logger;
            this.roleManager = roleManager;
        }

        [TempData]
        public string Message { get; set; }

        // public List<FarmImage> farmsListWithImages;

        // public List<CustomerComment> farmListRating;

        // public List<FarmImage> FarmsImagesList;

        // public List<Farm> FarmsList;

        public void OnGet()
        {
            // FIXME: If time at the end, try to do it this way
            // farmsListWithImages = db.FarmImages.Include(f => f.Farm).ToList();
            // farmListRating = db.CustomerComments.Include(f => f.Farm).ToList();
            // FarmsImagesList = db.FarmImages.ToList();
            // FarmsList = db.Farms.FromSqlRaw("SELECT FarmImages.ImageName as ImagePath, Farms.Name, Farms.Address FROM Farms INNER JOIN FarmImages ON FarmImages.FarmId  == Farms.Id INNER JOIN CustomerComments ON CustomerComments.FarmId == Farms.Id ORDER BY CustomerComments.Rating DESC LIMIT 3").ToList();

            //TODO: Don't forget to calculate average for top rated farms!!


            // FarmsImagesList = db.FarmImages.FromSqlRaw("SELECT * FROM Farms INNER JOIN FarmImages ON FarmImages.FarmId == Farms.Id INNER JOIN CustomerComments ON CustomerComments.FarmId == Farms.Id ORDER BY CustomerComments.Rating DESC LIMIT 3").ToList();
        }
        public void OnPost()
        { }
    }
}
