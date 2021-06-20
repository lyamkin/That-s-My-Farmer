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

namespace MyFarmer.Pages.Admin
{
    [Authorize(Roles = "Admin")]
    public class FarmServiceModel : PageModel
    {
        private readonly MyFarmerDbContext db;
        private readonly ILogger<UsersModel> logger;
        private readonly UserManager<IdentityUser> userManager;
        private readonly SignInManager<IdentityUser> signInManager;

        private readonly RoleManager<IdentityRole> roleManager;

        public FarmServiceModel(MyFarmerDbContext db, UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager, ILogger<UsersModel> logger, RoleManager<IdentityRole> roleManager)
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
        public InputModel Input { get; set; }

        public class InputModel
        {
            [Display(Name = "Farm Service Id")]
            public int FarmServiceId { get; set; }


            public string FarmName { get; set; }


            public string ServiceDescription { get; set; }

            [Display(Name = "Comments")]
            public string Comments { get; set; }

        }

        [TempData]
        [BindProperty]
        public string Message { get; set; }


        public List<Farm> FarmList = new List<Farm>();

        public List<Service> ServiceList = new List<Service>();

        public List<FarmsService> FarmServiceList = new List<FarmsService>();

        public void OnGet()
        {
            FarmList = db.Farms.ToList();
            ServiceList = db.Servises.ToList();
            FarmServiceList = db.FarmsServices.ToList();
            Input = new InputModel();
        }

        public IActionResult OnPost(int id)
        {
            var farmService = db.FarmsServices.Include(f => f.Farm).Include(f => f.FarmService).FirstOrDefault(i => i.Id == id);
            if (farmService != null)
            {
                Input = new InputModel();
                Input.FarmServiceId = farmService.Id;
                Input.FarmName = farmService.Farm.Name;
                Input.ServiceDescription = farmService.FarmService.Description;
                Input.Comments = farmService.Comments;
            }

            FarmList = db.Farms.ToList();
            ServiceList = db.Servises.ToList();
            FarmServiceList = db.FarmsServices.ToList();
            ModelState.Clear();
            return Page();
        }

        public IActionResult OnGetClearForm()
        {
            return RedirectToPage("/Admin/FarmService");
        }

        public IActionResult OnPostAddFarmService()
        {
            if (ModelState.IsValid)
            {
                FarmsService newFarmService = new FarmsService();
                newFarmService.FarmService = db.Servises.FirstOrDefault(fs => fs.Description == Input.ServiceDescription);
                newFarmService.Farm = db.Farms.FirstOrDefault(fs => fs.Name == Input.FarmName);
                newFarmService.Comments = Input.Comments;
                db.FarmsServices.Add(newFarmService);
                db.SaveChanges();
                logger.LogInformation($"Farm {Input.FarmName} with Service {Input.ServiceDescription} was added to database");
                Message = $"Farm {Input.FarmName} with Service {Input.ServiceDescription} was created";
                return RedirectToPage("/Admin/FarmService");
            }
            else
            {
                FarmList = db.Farms.ToList();
                ServiceList = db.Servises.ToList();
                FarmServiceList = db.FarmsServices.ToList();
                return Page();
            }
        }

        public IActionResult OnPostUpdateFarmService(int id)
        {
            if (ModelState.IsValid)
            {
                var updatedFarmService = db.FarmsServices.Include(f => f.Farm).Include(f => f.FarmService).FirstOrDefault(i => i.Id == id);
                if (updatedFarmService != null)
                {
                    updatedFarmService.Farm = db.Farms.FirstOrDefault(f => f.Name == Input.FarmName);
                    updatedFarmService.FarmService = db.Servises.FirstOrDefault(s => s.Description == Input.ServiceDescription);
                    updatedFarmService.Comments = Input.Comments;
                    db.FarmsServices.Update(updatedFarmService);
                    db.SaveChanges();
                    logger.LogInformation($"Farm {Input.FarmName} with Service {Input.ServiceDescription} was updated");
                    Message = $"Farm {Input.FarmName} with Service {Input.ServiceDescription} was updated";
                }
                return RedirectToPage("/Admin/FarmService");

            }
            else
            {
                FarmList = db.Farms.ToList();
                ServiceList = db.Servises.ToList();
                FarmServiceList = db.FarmsServices.ToList();
                return Page();
            }

        }

    }
}
