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
    public class FarmFoodModel : PageModel
    {
        private readonly MyFarmerDbContext db;
        private readonly ILogger<UsersModel> logger;
        private readonly UserManager<IdentityUser> userManager;
        private readonly SignInManager<IdentityUser> signInManager;

        private readonly RoleManager<IdentityRole> roleManager;

        public FarmFoodModel(MyFarmerDbContext db, UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager, ILogger<UsersModel> logger, RoleManager<IdentityRole> roleManager)
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
            [Display(Name = "Farm Food Id")]
            public int FarmFoodId { get; set; }


            public string FarmName { get; set; }


            public string FoodDescription { get; set; }

            [Display(Name = "Comments")]
            public string Comments { get; set; }

        }

        [TempData]
        [BindProperty]
        public string Message { get; set; }


        public List<Farm> FarmList = new List<Farm>();

        public List<Food> FoodList = new List<Food>();

        public List<FarmsFood> FarmFoodList = new List<FarmsFood>();

        public void OnGet()
        {
            FarmList = db.Farms.ToList();
            FoodList = db.Foods.ToList();
            FarmFoodList = db.FarmsFoods.ToList();
            Input = new InputModel();
        }

        public IActionResult OnPost(int id)
        {
            var farmFood = db.FarmsFoods.Include(f => f.Farm).Include(f => f.FarmFood).FirstOrDefault(i => i.Id == id);
            if (farmFood != null)
            {
                Input = new InputModel();
                Input.FarmFoodId = farmFood.Id;
                Input.FarmName = farmFood.Farm.Name;
                Input.FoodDescription = farmFood.FarmFood.Description;
                Input.Comments = farmFood.Comments;
            }
            FarmList = db.Farms.ToList();
            FoodList = db.Foods.ToList();
            FarmFoodList = db.FarmsFoods.ToList();
            ModelState.Clear();
            return Page();
        }

        public IActionResult OnGetClearForm()
        {
            return RedirectToPage("/Admin/FarmFood");
        }

        public IActionResult OnPostAddFarmFood()
        {
            if (ModelState.IsValid)
            {
                FarmsFood newFarmFood = new FarmsFood();
                newFarmFood.FarmFood = db.Foods.FirstOrDefault(fs => fs.Description == Input.FoodDescription);
                newFarmFood.Farm = db.Farms.FirstOrDefault(fs => fs.Name == Input.FarmName);
                newFarmFood.Comments = Input.Comments;
                db.FarmsFoods.Add(newFarmFood);
                db.SaveChanges();
                logger.LogInformation($"Farm {Input.FarmName} with Food {Input.FoodDescription} was added to database");
                Message = $"Farm {Input.FarmName} with Food {Input.FoodDescription} was created";
                return RedirectToPage("/Admin/FarmFood");
            }
            else
            {
                FarmList = db.Farms.ToList();
                FoodList = db.Foods.ToList();
                FarmFoodList = db.FarmsFoods.ToList();
                return Page();
            }
        }

        public IActionResult OnPostUpdateFarmFood(int id)
        {
            if (ModelState.IsValid)
            {
                var updatedFarmFood = db.FarmsFoods.Include(f => f.Farm).Include(f => f.FarmFood).FirstOrDefault(i => i.Id == id);
                if (updatedFarmFood != null)
                {
                    updatedFarmFood.Farm = db.Farms.FirstOrDefault(f => f.Name == Input.FarmName);
                    updatedFarmFood.FarmFood = db.Foods.FirstOrDefault(s => s.Description == Input.FoodDescription);
                    updatedFarmFood.Comments = Input.Comments;
                    db.FarmsFoods.Update(updatedFarmFood);
                    db.SaveChanges();
                    logger.LogInformation($"Farm {Input.FarmName} with Food {Input.FoodDescription} was updated");
                    Message = $"Farm {Input.FarmName} with Food {Input.FoodDescription} was updated";
                }
                return RedirectToPage("/Admin/FarmFood");

            }
            else
            {
                FarmList = db.Farms.ToList();
                FoodList = db.Foods.ToList();
                FarmFoodList = db.FarmsFoods.ToList();
                return Page();
            }

        }

    }
}
