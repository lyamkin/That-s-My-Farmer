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
    public class FarmsModel : PageModel
    {
        private readonly MyFarmerDbContext db;
        private readonly ILogger<UsersModel> logger;
        private readonly UserManager<IdentityUser> userManager;
        private readonly SignInManager<IdentityUser> signInManager;

        private readonly RoleManager<IdentityRole> roleManager;
        public FarmsModel(MyFarmerDbContext db, UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager, ILogger<UsersModel> logger, RoleManager<IdentityRole> roleManager)
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
        public InputModel Input { get; set; }

        public class InputModel
        {
            public int FarmId { get; set; }

            [Required]
            [Display(Name = "Farmer Name")]
            public IdentityUser Farmer { get; set; }

            [Required]
            [Display(Name = "Farm Name")]
            public string Name { get; set; }

            [Required]
            [EmailAddress]
            [Display(Name = "Email")]
            public string Email { get; set; }

            [Required]
            [DataType(DataType.PhoneNumber)]
            [Display(Name = "Phone")]
            public string Phone { get; set; }

            [Required]
            [Display(Name = "Farm Address")]
            public string Address { get; set; }

            [Required]
            [Display(Name = "Farm Description")]
            public string Description { get; set; }
        }

        public List<Farm> FarmList = new List<Farm>();

        [TempData]
        [BindProperty]
        public string Message { get; set; }

        public List<IdentityUser> FarmersList = new List<IdentityUser>();

        public List<IdentityUser> UserList = new List<IdentityUser>();

        public void OnGet()
        {
            FarmList = db.Farms.Include(m => m.Farmer).ToList();
            UserList = userManager.Users.ToList();
            foreach (IdentityUser user in UserList)
            {
                if (userManager.IsInRoleAsync(user, "Farmer").Result)
                {
                    FarmersList.Add(user);
                }
            }
            Input = new InputModel();

        }

        public IActionResult OnPost(int id)
        {
            var farm = db.Farms.Include(m => m.Farmer).FirstOrDefault(i => i.Id == id);
            Input = new InputModel();
            Input.FarmId = farm.Id;
            Input.Farmer = farm.Farmer;
            Input.Name = farm.Name;
            Input.Email = farm.Email;
            Input.Phone = farm.Phone;
            Input.Address = farm.Address;
            Input.Description = farm.Description;
            FarmList = db.Farms.Include(m => m.Farmer).ToList();
            UserList = userManager.Users.ToList();
            foreach (IdentityUser user in UserList)
            {
                if (userManager.IsInRoleAsync(user, "Farmer").Result)
                {
                    FarmersList.Add(user);
                }
            }
            ModelState.Clear();
            return Page();
        }

        public IActionResult OnGetClearForm()
        {
            return RedirectToPage("/Admin/Farms");
        }

        public async Task<IActionResult> OnPostAddFarm()
        {
            if (ModelState.IsValid)
            {
                Farm newFarm = new Farm();
                newFarm.Farmer = await userManager.FindByNameAsync(Input.Farmer.UserName);
                newFarm.Name = Input.Name;
                newFarm.Email = Input.Email;
                newFarm.Phone = Input.Phone;
                newFarm.Address = Input.Address;
                newFarm.Description = Input.Description;
                db.Farms.Add(newFarm);
                db.SaveChanges();
                logger.LogInformation($"Farm {Input.Name} was added to database");
                Message = $"Farm {Input.Name} was created";
                return RedirectToPage("/Admin/Farms");

            }
            else
            {
                FarmList = db.Farms.Include(m => m.Farmer).ToList();
                UserList = userManager.Users.ToList();
                foreach (IdentityUser user in UserList)
                {
                    if (userManager.IsInRoleAsync(user, "Farmer").Result)
                    {
                        FarmersList.Add(user);
                    }
                }
                return Page();
            }
        }

        public async Task<IActionResult> OnPostUpdateFarm(int id)
        {
            if (ModelState.IsValid)
            {
                Farm updatedFarm = db.Farms.Where(farm => farm.Id == id).FirstOrDefault();
                updatedFarm.Farmer = await userManager.FindByNameAsync(Input.Farmer.UserName);
                updatedFarm.Name = Input.Name;
                updatedFarm.Email = Input.Email;
                updatedFarm.Phone = Input.Phone;
                updatedFarm.Address = Input.Address;
                updatedFarm.Description = Input.Description;
                db.Farms.Update(updatedFarm);
                db.SaveChanges();
                logger.LogInformation($"Farm {Input.Name} was update");
                Message = $"Farm {Input.Name} was updated";
                return RedirectToPage("/Admin/Farms");
            }
            else
            {
                FarmList = db.Farms.Include(m => m.Farmer).ToList();
                UserList = userManager.Users.ToList();
                foreach (IdentityUser user in UserList)
                {
                    if (userManager.IsInRoleAsync(user, "Farmer").Result)
                    {
                        FarmersList.Add(user);
                    }
                }
                return Page();
            }

        }



    }
}
