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
    public class ServiceModel : PageModel
    {
        private readonly MyFarmerDbContext db;
        private readonly ILogger<UsersModel> logger;
        private readonly UserManager<IdentityUser> userManager;
        private readonly SignInManager<IdentityUser> signInManager;

        private readonly RoleManager<IdentityRole> roleManager;

        public ServiceModel(MyFarmerDbContext db, UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager, ILogger<UsersModel> logger, RoleManager<IdentityRole> roleManager)
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
            [Display(Name = "Service Id")]
            public int ServiceId { get; set; }

            [Required]
            [Display(Name = "Description")]
            public string Description { get; set; }

        }

        [TempData]
        [BindProperty]
        public string Message { get; set; }


        public List<Service> ServiceList = new List<Service>();

        public void OnGet()
        {
            ServiceList = db.Servises.ToList();
            Input = new InputModel();
        }

        public IActionResult OnPost(int id)
        {
            var service = db.Servises.FirstOrDefault(i => i.Id == id);
            Input = new InputModel();
            Input.ServiceId = service.Id;
            Input.Description = service.Description;
            ServiceList = db.Servises.ToList();
            ModelState.Clear();
            return Page();
        }

        public IActionResult OnGetClearForm()
        {
            return RedirectToPage("/Admin/Service");
        }

        public IActionResult OnPostAddService()
        {
            Service uniqueDescription = db.Servises.FirstOrDefault(f => f.Description == Input.Description);
            if (uniqueDescription != null)
            {
                ModelState.AddModelError("", $" Service {Input.Description} already exists in the database.");
                ServiceList = db.Servises.ToList();
                return Page();
            }
            if (ModelState.IsValid)
            {
                Service newService = new Service();
                newService.Description = Input.Description;
                db.Servises.Add(newService);
                db.SaveChanges();
                logger.LogInformation($"Service {Input.Description} was added to database");
                Message = $"Service {Input.Description} was created";
                return RedirectToPage("/Admin/Service");
            }
            else
            {
                ServiceList = db.Servises.ToList();
                return Page();
            }
        }

        public IActionResult OnPostUpdateService(int id)
        {
            Service uniqueDescription = db.Servises.FirstOrDefault(f => f.Description == Input.Description);
            if (uniqueDescription != null)
            {
                ModelState.AddModelError("", $" Service {Input.Description} already exists in the database.");
                ServiceList = db.Servises.ToList();
                return Page();
            }
            if (ModelState.IsValid)
            {
                Service updatedService = db.Servises.Where(service => service.Id == id).FirstOrDefault();
                updatedService.Description = Input.Description;
                db.Servises.Update(updatedService);
                db.SaveChanges();
                logger.LogInformation($"Service {Input.Description} was update");
                Message = $"Service {Input.Description} was updated";
                return RedirectToPage("/Admin/Service");
            }
            else
            {
                ServiceList = db.Servises.ToList();
                return Page();
            }
          
        }

    }
}
