using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using MyFarmer.Models;

namespace MyFarmer.Pages.Account
{
    public class RegisterModel : PageModel
    {
        private readonly UserManager<IdentityUser> userManager;
        private readonly SignInManager<IdentityUser> signInManager;
        private readonly ILogger<RegisterModel> logger;

        public RegisterModel(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager,ILogger<RegisterModel> logger)
         {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.logger = logger;
        }
        [BindProperty]
        public InputModel Input { get; set; }
        public string Message { get; set; }

        public class InputModel
        {
            [Required]
            [EmailAddress]
            [RegularExpression(@"^[a-zA-Z0-9_.+-]+@[a-zA-Z0-9-]+\.[a-zA-Z0-9-.]+$", ErrorMessage ="Invalid Email Format")]
            public string Email { get; set; }

            [Required]
            [StringLength(100, ErrorMessage ="The {0} must be at least {2} and max {1} characters long.",MinimumLength = 6)]
            [DataType(DataType.Password)]
            public string Password {get;set;}

            [DataType(DataType.Password)]
            [Display(Name ="Confirm password")]
            [Compare("Password", ErrorMessage ="Password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; }
            [Display(Name = "Register me as a Farmer")]
            [BindProperty]
            public bool RegisterMeAsFarmer { get; set; }
        }

        public void OnGet()
        {
        }
        public async Task<IActionResult> OnPostAsync() {
            if(ModelState.IsValid) 
            {
                // Create new object from IdentityUser
                var user = new IdentityUser {UserName = Input.Email, Email = Input.Email, EmailConfirmed = true}; 
                // Create new user in the AspNetUsers table
                var createUserResult = await userManager.CreateAsync(user, Input.Password);
                if(createUserResult.Succeeded)
                {
                    // Try to add "User" or Farmer Role to the new user
                    IdentityResult addRoleResult = new IdentityResult();
                    if(Input.RegisterMeAsFarmer) 
                    {
                        addRoleResult = await userManager.AddToRoleAsync(user, "Farmer");
                    } else 
                    {
                        addRoleResult = await userManager.AddToRoleAsync(user, "User");
                    }
                    
                    // If Role assigned successful than move to the login
                    if(addRoleResult.Succeeded) 
                    {
                        logger.LogInformation($"The new user {Input.Email} was created");
                        TempData["message"] = $"The new user {Input.Email} was created"; // save message in temporary storage

                        // redirect to login page
                        return RedirectToPage("Login");
                    } else 
                    // If Role was not assigned delete login and redirect to Index
                    {
                        // Delete user because role was not created and save the message
                        logger.LogInformation($"The user {Input.Email} was deleted because some issues in role creation");
                        TempData["message"] = $"The user {Input.Email} was deleted because some issues in role creation";
                        var deleteUserResult = userManager.DeleteAsync(user);
                        // If some issues with role creation redirect to index and display error
                        return RedirectToPage("/Index");
                    }
                }
                // If user creation failed then save all errors
                foreach(var error in createUserResult.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                    logger.LogInformation($"User creation issue: {error.Description}");
                }
            }
            return Page();
        }
    }
}
