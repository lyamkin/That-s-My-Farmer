using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MyFarmer.Data;
using MyFarmer.Models;

namespace MyFarmer.Pages
{
    [Authorize(Roles = "Farmer")]
    public class DeleteFarmModel : PageModel
    {
        private readonly MyFarmerDbContext db;
        private readonly ILogger<DeleteFarmModel> logger;
        public DeleteFarmModel(MyFarmerDbContext db, ILogger<DeleteFarmModel> logger)
        {
            this.db = db;
            this.logger = logger;
        }
        [BindProperty]
        public Farm MyFarm {get;set;}
        [TempData]
        public string Message { get; set; }
        [BindProperty(SupportsGet =true)]
        public int Id { get; set; }
        public IActionResult OnGet()
        {
            //Get the Farm by Id
            MyFarm = db.Farms.FirstOrDefault(Farm=>Farm.Id==Id);
            if(MyFarm == null)
            {
                return RedirectToPage("/NotFound");
            }
            return Page();
        }
        public IActionResult OnPost()
        {
            Farm farmToDelete = db.Farms.FirstOrDefault(Farm=>Farm.Id==Id);
            if (farmToDelete == null)
            {
                logger.LogInformation($"Farm with is {Id} was not found.");
                return RedirectToPage("/NotFound");
            }
            //TODO: Step back if one of transactions failed 
            // Get all records from FarmsFood and delete them
            IList<FarmsFood> farmsFoodToDelete = db.FarmsFoods.Where(item => item.Farm == farmToDelete).ToList();
            db.FarmsFoods.RemoveRange(farmsFoodToDelete);
            // Get all records from FarmsService and delete them
            IList<FarmsService> farmsServicesToDelete = db.FarmsServices.Where(item => item.Farm == farmToDelete).ToList();
            db.FarmsServices.RemoveRange(farmsServicesToDelete);
            // Get all records from FarmImage and delete them
            IList<FarmImage> farmsImagesToDelete = db.FarmImages.Where(item => item.Farm == farmToDelete).ToList();
            db.FarmImages.RemoveRange(farmsImagesToDelete);
            // Remove the farm
            db.Farms.Remove(farmToDelete);
            db.SaveChanges();
            TempData["message"] = $"The farm {farmToDelete.Name} was deleted";
            return RedirectToPage("/FarmsList");
        }
    }
}

