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
    public class FoodModel : PageModel
    {
        private readonly MyFarmerDbContext db;
        private readonly ILogger<UsersModel> logger;
        private readonly UserManager<IdentityUser> userManager;
        private readonly SignInManager<IdentityUser> signInManager;

        private readonly RoleManager<IdentityRole> roleManager;

        public FoodModel(MyFarmerDbContext db, UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager, ILogger<UsersModel> logger, RoleManager<IdentityRole> roleManager)
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
            [Display(Name = "Food Id")]
            public int FoodId { get; set; }

            [Required]
            [Display(Name = "Description")]
            public string Description { get; set; }

        }

        [TempData]
        [BindProperty]
        public string Message { get; set; }


        public List<Food> FoodList = new List<Food>();

        public void OnGet()
        {
            FoodList = db.Foods.ToList();
            Input = new InputModel();
        }

        public IActionResult OnPost(int id)
        {
            var food = db.Foods.FirstOrDefault(i => i.Id == id);
            Input = new InputModel();
            Input.FoodId = food.Id;
            Input.Description = food.Description;
            FoodList = db.Foods.ToList();
            ModelState.Clear();
            return Page();
        }

        public IActionResult OnGetClearForm()
        {
            return RedirectToPage("/Admin/Food");
        }

        public IActionResult OnPostAddFood()
        {
            Food uniqueDescription = db.Foods.FirstOrDefault(f => f.Description == Input.Description);
            if (uniqueDescription != null)
            {
                ModelState.AddModelError("", $" Food {Input.Description} already exists in the database.");
                FoodList = db.Foods.ToList();
                return Page();
            }
            if (ModelState.IsValid)
            {
                Food newFood = new Food();
                newFood.Description = Input.Description;
                db.Foods.Add(newFood);
                db.SaveChanges();
                logger.LogInformation($"Food {Input.Description} was added to database");
                Message = $"Food {Input.Description} was created";
                return RedirectToPage("/Admin/Food");
            }
            else
            {
                FoodList = db.Foods.ToList();
                return Page();
            }
        }

        public IActionResult OnPostUpdateFood(int id)
        {
            Food uniqueDescription = db.Foods.FirstOrDefault(f => f.Description == Input.Description);
            if (uniqueDescription != null)
            {
                ModelState.AddModelError("", $" Food {Input.Description} already exists in the database.");
                FoodList = db.Foods.ToList();
                return Page();
            }
            if (ModelState.IsValid)
            {
                Food updatedFood = db.Foods.Where(food => food.Id == id).FirstOrDefault();
                updatedFood.Description = Input.Description;
                db.Foods.Update(updatedFood);
                db.SaveChanges();
                logger.LogInformation($"Food {Input.Description} was update");
                Message = $"Food {Input.Description} was updated";
                return RedirectToPage("/Admin/Food");
            }
            else
            {
                FoodList = db.Foods.ToList();
                return Page();
            }

        }
    }
}
