using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MyFarmer.Data;
using MyFarmer.Models;

namespace MyFriends2.Pages
{
    public class RegisterFarmerModel : PageModel
    {
        private readonly UserManager<IdentityUser> userManager;
        private readonly SignInManager<IdentityUser> signInManager;
        private readonly ILogger<RegisterFarmerModel> logger;
        private readonly MyFarmerDbContext db;

        public RegisterFarmerModel(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager,ILogger<RegisterFarmerModel> logger, MyFarmerDbContext db)
         {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.logger = logger;
            this.db = db;
        }
        // [BindProperty(SupportsGet =true)]
        //     public string Email { get; set; }
        // [BindProperty]
        // public InputModel Input { get; set; }
        // public class InputModel
        // {
        //     public string Name { get; set; }
        //     public string Phone { get; set; }
        //     public string Address { get; set; }
        //     public string Description { get; set; }
        // }
        [TempData]
        public string Message { get; set; }
        [BindProperty]
        public Farm NewFarm {get; set;} = new Farm();

        public async Task<IActionResult> OnGetAsync(string email)
        {
            if (email == null)
            {
                return NotFound();
            }
            NewFarm.Email = email; // save email
            // TODO: Be sure the there are user in the db
            //NewFarm.Farmer = await userManager.FindByEmailAsync(email); // find user by email and save it
            // FIX ISSUE  FindByEmailAsync retuurn email not object
            var normalizerEmail = userManager.NormalizeEmail(email);
            var user = await userManager.FindByEmailAsync(normalizerEmail); // find user by email and save it
            logger.LogInformation($"New object : {user}");
            return Page();
            
        }
        public async Task<IActionResult> OnPostAsync() {
            if(!ModelState.IsValid) 
            {
               return Page();
            }
            //TODO if farmer not created delete account
            db.Farms.Add(NewFarm); // Add new farm into database
            // Try to save changes
            
            await db.SaveChangesAsync();
            TempData["message"] = $"The new Farmer {NewFarm.Name} was created"; // save message in temporary storage
            logger.LogInformation($"The new Farmer {NewFarm.Name} was created");
            return RedirectToPage("Login"); 
        }
    }
}
