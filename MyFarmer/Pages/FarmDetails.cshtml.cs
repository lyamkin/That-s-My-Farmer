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

namespace MyFarmer.Pages
{
    [Authorize]
    public class FarmDetailsModel : PageModel
    {
        private readonly MyFarmerDbContext db;
        private readonly ILogger<FarmDetailsModel> logger;
        private readonly SignInManager<IdentityUser> signInManager;
        private readonly UserManager<IdentityUser> userManager;

        public FarmDetailsModel(MyFarmerDbContext db, ILogger<FarmDetailsModel> logger, SignInManager<IdentityUser> signInManager, UserManager<IdentityUser> userManager)
        {
            this.db = db;
            this.logger = logger;
            this.signInManager = signInManager;
            this.userManager = userManager;
        }
        // Farm Id
        [BindProperty(SupportsGet = true)]
        public int Id { get; set; }
        [TempData]
        public string Message { get; set; }

        public InputModelFarm InputFarm { get; set; }

        public class InputModelFarm
        {
            public int FarmId { get; set; }


            [Display(Name = "Farmer Name")]
            public IdentityUser Farmer { get; set; }


            [Display(Name = "Farm Name")]
            public string Name { get; set; }


            [EmailAddress]
            [Display(Name = "Email")]
            public string Email { get; set; }


            [DataType(DataType.PhoneNumber)]
            [Display(Name = "Phone")]
            public string Phone { get; set; }

            [Required]
            [Display(Name = "Farm Address")]
            public string Address { get; set; }


            [Display(Name = "Farm Description")]
            public string Description { get; set; }
        }

        [BindProperty]
        public InputModelAddReview InputReview { get; set; }

        public class InputModelAddReview
        {
            public IdentityUser Customer { get; set; }

            public string FarmName { get; set; }

            [DataType(DataType.DateTime)]
            public DateTime Created { get; set; }

            [Required, MinLength(1), MaxLength(2000)]
            [Display(Name = "Post Content:")]
            public string Message { get; set; }

            [Range(0, 5, ErrorMessage = "Rating must be between 0-5")]
            [Display(Name = "Rating / 5:")]
            public int Rating { get; set; }

        }
        public List<FarmsFood> FarmsFoods = new List<FarmsFood>();
        public List<FarmsService> FarmsServices = new List<FarmsService>();
        public List<FarmImage> FarmsImages = new List<FarmImage>();

        public List<CustomerComment> custCommentsList { get; set; } = new List<CustomerComment>();

        public IdentityUser CurrentUser;
        public IActionResult OnGet()
        {
            custCommentsList = db.CustomerComments.Include(f => f.Farm).Include(c => c.Customer).OrderByDescending(c => c.Created).ToList();
            InputReview = new InputModelAddReview();
            CurrentUser = userManager.FindByNameAsync(User.Identity.Name).Result;
            InputReview.Customer = CurrentUser;
            // Try to get Farm By Id
            var farm = db.Farms.Include(farm => farm.Farmer).FirstOrDefault(farm => farm.Id == Id);
            if (farm == null)
            {
                logger.LogError($"Can't find farm with Id:{Id}");
                return RedirectToPage("/NotFound");
            }
            InputReview.FarmName = farm.Name;
            // If farm exist than get data
            InputFarm = new InputModelFarm();
            InputFarm.FarmId = farm.Id;
            InputFarm.Farmer = farm.Farmer;
            InputFarm.Name = farm.Name;
            InputFarm.Email = farm.Email;
            InputFarm.Phone = farm.Phone;
            InputFarm.Address = farm.Address;
            InputFarm.Description = farm.Description;
            // Get Food list based on Farm Id
            FarmsFoods = db.FarmsFoods.Include(farm => farm.Farm).Include(farm => farm.FarmFood).Where(farm => farm.Farm.Id == Id).ToList();
            // Get Services list based on Farm Id
            FarmsServices = db.FarmsServices.Include(farm => farm.Farm).Include(farm => farm.FarmService).Where(farm => farm.Farm.Id == Id).ToList();
            // Get Services list based on Farm Id
            FarmsImages = db.FarmImages.Include(farm => farm.Farm).Where(farm => farm.Farm.Id == Id).ToList();
            return Page();
        }
        public async Task<IActionResult> OnPostReview()
        {
            if (ModelState.IsValid)
            {
                CustomerComment newPost = new CustomerComment();
                newPost.Customer = userManager.FindByNameAsync(User.Identity.Name).Result;
                // Try to get Farm By Id
                var farm = db.Farms.Include(farm => farm.Farmer).FirstOrDefault(farm => farm.Id == Id);
                newPost.Farm = farm;
                InputReview.Created = DateTime.Now;
                newPost.Created = InputReview.Created;
                newPost.Message = InputReview.Message;
                newPost.Rating = InputReview.Rating;
                db.CustomerComments.Add(newPost);
                db.SaveChanges();
                logger.LogInformation($"Post was successfully added to the database");
                Message = $"Review was successfully added.";
                return RedirectToPage("/FarmDetails");

            }
            else
            {
                InputReview = new InputModelAddReview();
                CurrentUser = userManager.Users.FirstOrDefault(i => i.UserName == @User.Identity.Name);
                InputReview.Customer = CurrentUser;
                custCommentsList = db.CustomerComments.Include(f => f.Farm).Include(c => c.Customer).OrderByDescending(c => c.Created).ToList();

                // Try to get Farm By Id
                var farm = db.Farms.Include(farm => farm.Farmer).FirstOrDefault(farm => farm.Id == Id);
                if (farm == null)
                {
                    logger.LogError($"Can't find farm with Id:{Id}");
                    return RedirectToPage("/NotFound");
                }
                InputReview.FarmName = farm.Name;
                // If farm exist than get data
                InputFarm = new InputModelFarm();
                InputFarm.FarmId = farm.Id;
                InputFarm.Farmer = farm.Farmer;
                InputFarm.Name = farm.Name;
                InputFarm.Email = farm.Email;
                InputFarm.Phone = farm.Phone;
                InputFarm.Address = farm.Address;
                InputFarm.Description = farm.Description;

                // Get Food list based on Farm Id
                FarmsFoods = db.FarmsFoods.Include(farm => farm.Farm).Include(farm => farm.FarmFood).Where(farm => farm.Farm.Id == Id).ToList();
                // Get Services list based on Farm Id
                FarmsServices = db.FarmsServices.Include(farm => farm.Farm).Include(farm => farm.FarmService).Where(farm => farm.Farm.Id == Id).ToList();
                // Get Services list based on Farm Id
                FarmsImages = db.FarmImages.Include(farm => farm.Farm).Where(farm => farm.Farm.Id == Id).ToList();

                logger.LogInformation($"Error adding post to database");
                Message = $"An error occured while adding your post.";
                return Page();
            }
        }
    }

}
