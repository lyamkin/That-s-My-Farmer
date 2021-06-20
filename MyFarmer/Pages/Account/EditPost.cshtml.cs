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

namespace MyFarmer.Pages.Account
{
    [Authorize(Roles = "User")]
    public class EditPostModel : PageModel
    {

        [BindProperty(SupportsGet = true)]
        public int Id { get; set; }

        [BindProperty]
        public InputModel Input { get; set; }

        [TempData]
        [BindProperty]
        public string Message { get; set; }
        private readonly MyFarmerDbContext db;
        private readonly ILogger<EditUserModel> logger;
        private readonly UserManager<IdentityUser> userManager;
        private readonly SignInManager<IdentityUser> signInManager;

        private readonly RoleManager<IdentityRole> roleManager;

        public class InputModel
        {
            public int postId { get; set; }

            // [Required]
            // [Display(Name = "Username:")]
            // public IdentityUser Customer { get; set; }

            [Display(Name = "Posted By::")]
            public string Username;

            // [Required]
            // [Display(Name = "Farm Name: ")]
            // public Farm Farm { get; set; }

            [Display(Name = "Farm Name:")]
            public string FarmName;

            // [Required]
            [Display(Name = "Date Posted:")]
            [DataType(DataType.DateTime)]
            public DateTime Created { get; set; }

            [Required, MinLength(1), MaxLength(2000)]
            [Display(Name = "Post Content:")]
            public string Message { get; set; }

            [Range(0, 5, ErrorMessage = "Rating must be between 0-5")]
            [Display(Name = "Rating:")]
            public int Rating { get; set; }

        }

        public EditPostModel(MyFarmerDbContext db, UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager, ILogger<EditUserModel> logger, RoleManager<IdentityRole> roleManager)
        {
            this.db = db;
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.logger = logger;
            this.roleManager = roleManager;
        }

        public CustomerComment post;
        public Farm farm;

        public void OnGet()
        {
            Input = new InputModel();
            post = db.CustomerComments.Include(f => f.Farm).FirstOrDefault(p => p.Id == Id);
            farm = post.Farm;
            // FIXME: linq query failed
            // farm = db.CustomerComments.Include(f => f.Farm.Id == )
            // FarmList = db.Farms.Include(farm => farm.Farmer).Where(farm => farm.Farmer.Id == user.Id).ToList();

            Input.postId = post.Id;
            Input.Username = @User.Identity.Name;
            Input.FarmName = farm.Name;
            Input.Created = post.Created;
            Input.Message = post.Message;
            Input.Rating = post.Rating;
            // db.Frams.FromSQLRAw
        }

        public IActionResult OnPost()
        {
            if (ModelState.IsValid)
            {
                // retrieve post to update
                var postUpdate = db.CustomerComments.Include(f => f.Farm).Include(u => u.Customer).FirstOrDefault(p => p.Id == Id);
                if (postUpdate != null)
                {
                    postUpdate.Created = DateTime.Now;
                    postUpdate.Message = Input.Message;
                    postUpdate.Rating = Input.Rating;
                    db.CustomerComments.Update(postUpdate);
                    db.SaveChanges();
                    logger.LogInformation($"Post {Id} succesfully updated.");
                    Message = $"Post succesfully updated.";
                }
                return RedirectToPage("/Account/UserProfile");

            }
            else
            {

                post = db.CustomerComments.Include(f => f.Farm).FirstOrDefault(p => p.Id == Id);
                farm = post.Farm;


                Input.postId = post.Id;
                Input.Username = @User.Identity.Name;
                Input.FarmName = farm.Name;
                Input.Created = post.Created;
                Input.Message = post.Message;
                Input.Rating = post.Rating;
                return Page();
            }

        }


    }
}
