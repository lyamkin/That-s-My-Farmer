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
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Globalization;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace MyFarmer.Pages.Admin
{
    [Authorize(Roles = "Admin")]
    public class UsersModel : PageModel
    {
        private readonly MyFarmerDbContext db;
        private readonly ILogger<UsersModel> logger;
        private readonly UserManager<IdentityUser> userManager;
        private readonly SignInManager<IdentityUser> signInManager;

        private readonly RoleManager<IdentityRole> roleManager;

        public UsersModel(MyFarmerDbContext db, UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager, ILogger<UsersModel> logger, RoleManager<IdentityRole> roleManager)
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
            public string UserId { get; set; }

            [Required]
            [Display(Name = "UserName")]
            public string UserName { get; set; }

            [Required]
            [EmailAddress]
            [Display(Name = "Email")]
            public string Email { get; set; }

            [Required]
            [DataType(DataType.Password)]
            [Display(Name = "Password")]
            public string Password { get; set; }

            [Required]
            [DataType(DataType.Password)]
            [Display(Name = "Confirm password")]
            [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; }

            [Required]
            public string Role { get; set; }
        }

        public List<IdentityUser> UserList = new List<IdentityUser>();

        [TempData]
        [BindProperty]
        public string Message { get; set; }

        public List<IdentityRole> RolesList { get; set; }

        public void OnGet()
        {
            RolesList = roleManager.Roles.ToList();
            UserList = userManager.Users.ToList();
            Input = new InputModel();
            Input.UserId = "";
        }

        public IActionResult OnPost(string id)
        {
            var user = userManager.Users.FirstOrDefault(i => i.Id == id);
            Input = new InputModel();
            Input.UserId = user.Id;
            Input.UserName = user.UserName;
            Input.Email = user.Email;
            Input.Password = "FillInBlank123!";
            Input.ConfirmPassword = "FillInBlank123!";
            Input.Role = userManager.GetRolesAsync(user).Result.FirstOrDefault();
            UserList = userManager.Users.ToList();
            RolesList = roleManager.Roles.ToList();

            ModelState.Clear();
            
            return Page();

        }

        public async Task<IActionResult> OnPostAddUser()
        {
            if (ModelState.IsValid)
            {
                var user = new IdentityUser { UserName = Input.Email, Email = Input.Email, EmailConfirmed = true };
                var result = await userManager.CreateAsync(user, Input.Password);
                if (result.Succeeded)
                {
                    var result2 = await userManager.AddToRoleAsync(user, Input.Role);
                    if (result2.Succeeded)
                    {
                        logger.LogInformation($"User {Input.Email} create a new account with password");
                        Message = $"User {Input.Email} new account created";
                        return RedirectToPage("/Admin/Users");
                    }
                    else
                    {
                        // FIXED: delete the user since role assignment failed, log the event, show error to the user
                        await userManager.DeleteAsync(user);
                        logger.LogInformation($"User  {Input.Email} new account not created");
                        Message = $"User  {Input.Email} new account not created";
                        return RedirectToPage("/Admin/Users");
                    }
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                UserList = userManager.Users.ToList();
                return Page();
            }
            else
            {
                UserList = userManager.Users.ToList();
                return Page();
            }
        }

        public async Task<IActionResult> OnPostUpdateUser(string id)
        {
            if (ModelState.IsValid)
            {
                var user = userManager.Users.FirstOrDefault(i => i.Id == id);
                if (user != null)
                {
                    user.UserName = Input.Email;
                    user.Email = Input.Email;
                    var result = await userManager.UpdateAsync(user);
                    if (result.Succeeded)
                    {
                        var oldRole = userManager.GetRolesAsync(user).Result.FirstOrDefault();
                        if (oldRole != null)
                        {
                            await userManager.RemoveFromRoleAsync(user, oldRole);
                        }
                        var result2 = await userManager.AddToRoleAsync(user, Input.Role);
                        if (result2.Succeeded)
                        {
                            logger.LogInformation($"User {Input.Email} updated");
                            Message = $"User {Input.Email} account is updated";
                            return RedirectToPage("/Admin/Users");
                        }
                    }
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                    Input.UserId = id;
                    UserList = userManager.Users.ToList();
                    return Page();
                }
                return RedirectToPage("/Admin/Users");
            }
            Input.UserId = id;
            UserList = userManager.Users.ToList();
            return Page();
        }


    }
}
