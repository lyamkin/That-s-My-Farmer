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
    [Authorize(Roles = "User, Farmer")]
    public class EditUserModel : PageModel
    {

        [BindProperty(SupportsGet = true)]
        public string Id { get; set; }

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
            public string UserId { get; set; }

            [Required]
            [Display(Name = "Username:")]
            public string Username { get; set; }

            [Required]
            [EmailAddress]
            [Display(Name = "Email:")]
            public string Email { get; set; }

        }

        public EditUserModel(MyFarmerDbContext db, UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager, ILogger<EditUserModel> logger, RoleManager<IdentityRole> roleManager)
        {
            this.db = db;
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.logger = logger;
            this.roleManager = roleManager;
        }

        public IdentityUser currUser;

        public void OnGet()
        {
            Input = new InputModel();
            currUser = userManager.FindByNameAsync(User.Identity.Name).Result;

            Input.UserId = currUser.Id;
            Input.Email = currUser.Email;
            Input.Username = currUser.UserName;

        }

        // FIXME: user can't login after!!!!!
        public async Task<IActionResult> OnPost()
        {
            if (ModelState.IsValid)
            {
                // logger.LogInformation($"User {Input.UserId}");
                var user = userManager.FindByNameAsync(User.Identity.Name).Result;

                // if null, redirect
                if (user == null)
                {
                    return RedirectToPage("/Account/UserProfile");
                }

                user.UserName = Input.Email;
                user.Email = Input.Email;

                // sign user out and update
                await signInManager.SignOutAsync();
                var result = await userManager.UpdateAsync(user);

                // if result does not succeed, add errors to the model state and re-render the ChangePassword view
                if (!result.Succeeded)
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                    currUser = userManager.FindByNameAsync(User.Identity.Name).Result;

                    return Page();
                }

                //  refresh sign in cookie, sign user back in and re-direct user to UserProfile page
                await signInManager.RefreshSignInAsync(user);
                await signInManager.SignInAsync(user, false);
                Message = $"User {user.UserName} updated successfully.";
                return RedirectToPage("/Account/UserProfile");
            }

            // if modelState is not valid, re-render page
            currUser = userManager.FindByNameAsync(User.Identity.Name).Result;
            return Page();

        }
    }

}








//     if (ModelState.IsValid)
//     {
//         // logger.LogInformation($"User {Input.UserId}");
//         var user = userManager.Users.FirstOrDefault(i => i.Id == Input.UserId);
//         // verify if user not null
//         if (user != null)
//         {
//             user.UserName = Input.Username;
//             user.Email = Input.Email;
//             // verify password
//             // if (!string.IsNullOrEmpty(Input.Password))
//             // {
//             //     user.PasswordHash = "test123";
//             // }
//             // user.PasswordHash = currUser.PasswordHash;
//             await signInManager.SignOutAsync();
//             var result = await userManager.UpdateAsync(user);

//             // if success
//             if (result.Succeeded)
//             {
//                 await signInManager.SignInAsync(user, false);
//                 logger.LogInformation($"User {Input.Email} updated");
//                 Message = $"User {Input.Email} account information successfully updated";
//                 return RedirectToPage("/Account/UserProfile");
//             }
//             // else
//             foreach (var error in result.Errors)
//             {
//                 ModelState.AddModelError(string.Empty, error.Description);
//             }
//             Input.UserId = Id;
//             // Input.UserId = this.Id;
//             currUser = userManager.Users.FirstOrDefault(i => i.Id == Input.UserId);

//             Input.Email = currUser.Email;
//             Input.Username = currUser.UserName;
//             return Page();
//         }
//         logger.LogInformation($"User {Input.Email} NOT updated");
//         Message = $"User {Input.Email} an error occured while updating user information.";
//         return RedirectToPage("/Account/UserProfile");
//     }
//     Input.UserId = Id;
//     currUser = userManager.Users.FirstOrDefault(i => i.Id == Input.UserId);

//     Input.Email = currUser.Email;
//     Input.Username = currUser.UserName;
//     return Page();
// }
