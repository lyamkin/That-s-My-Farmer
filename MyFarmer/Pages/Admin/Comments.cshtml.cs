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
    public class CommentsModel : PageModel
    {
        private readonly MyFarmerDbContext db;
        private readonly ILogger<UsersModel> logger;
        private readonly UserManager<IdentityUser> userManager;
        private readonly SignInManager<IdentityUser> signInManager;

        private readonly RoleManager<IdentityRole> roleManager;
        public CommentsModel(MyFarmerDbContext db, UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager, ILogger<UsersModel> logger, RoleManager<IdentityRole> roleManager)
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
            public int CommentId { get; set; }

            [Required]
            [EmailAddress]
            [Display(Name = "User Email")]
            public string UserEmail { get; set; }

            [Required]
            [Display(Name = "Farm Name")]
            public string FarmName { get; set; }

            [Required]
            [Display(Name = "Date")]
            [DataType(DataType.Date)]
            public DateTime Date { get; set; }

            [Required]
            [Display(Name = "Message")]
            public string Message { get; set; }

            [Required]
            [Display(Name = "Rating")]
            public int Rating { get; set; }
        }

        [TempData]
        [BindProperty]
        public string Message { get; set; }

        public List<CustomerComment> CommentList = new List<CustomerComment>();
        public List<IdentityUser> UserList = new List<IdentityUser>();
         public List<Farm> FarmList = new List<Farm>();

        public void OnGet()
        {
            CommentList = db.CustomerComments.Include(u => u.Customer).Include(f => f.Farm).ToList();
            UserList = userManager.Users.OrderBy(u => u.Email).ToList();
            FarmList = db.Farms.OrderBy(f => f.Name).ToList();
            Input = new InputModel();
        }
    }
}
