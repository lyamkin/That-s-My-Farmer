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
using Google.DataTable.Net.Wrapper;
using Google.DataTable.Net.Wrapper.Extension;

namespace MyFarmer.Pages.Admin
{
    [Authorize(Roles = "Admin")]
    public class IndexModel : PageModel
    {
        private readonly MyFarmerDbContext db;
        private readonly ILogger<UsersModel> logger;
        private readonly UserManager<IdentityUser> userManager;
        private readonly SignInManager<IdentityUser> signInManager;

        private readonly RoleManager<IdentityRole> roleManager;

        public IndexModel(MyFarmerDbContext db, UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager, ILogger<UsersModel> logger, RoleManager<IdentityRole> roleManager)
        {
            this.db = db;
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.logger = logger;
            this.roleManager = roleManager;
        }

        [BindProperty]
        public int UsersTotal { get; set; }

        [BindProperty]
        public int FarmsTotal { get; set; }

        [BindProperty]
        public int FoodTotal { get; set; }

        [BindProperty]
        public int ServiceTotal { get; set; }

         [BindProperty]
        public int CommentTotal { get; set; }

        public class Chart
        {
            public object[] cols { get; set; }
            public object[] rows { get; set; }
        }

        public class UserRoleCount
        {
            public string Role { get; set; }
            public int Count { get; set; }
        }

        public List<UserRoleCount> UrcList = new List<UserRoleCount>();


        public void OnGet()
        {
            UsersTotal = userManager.Users.Count();
            FarmsTotal = db.Farms.Count();
            FoodTotal = db.FarmsFoods.Count();
            ServiceTotal = db.FarmsServices.Count();
            CommentTotal = db.CustomerComments.Count();
            UrcList = CountUserPerRole();
        }

        public IActionResult OnGetChartData()
        {
            UrcList = CountUserPerRole();
           var json = UrcList.ToGoogleDataTable()
               .NewColumn(new Column(ColumnType.String, "Role"), x => x.Role)
               .NewColumn(new Column(ColumnType.Number, "Count"), x => x.Count)
               .Build()
               .GetJson();

            return Content(json);
        }



        public List<UserRoleCount> CountUserPerRole()
        {
            var RolesList = roleManager.Roles;
            List<UserRoleCount> urcList = new List<UserRoleCount>();
            foreach (var r in RolesList)
            {
                string role = r.Name;
                var usersList = userManager.GetUsersInRoleAsync(r.Name).Result;
                int count = usersList.Count;
                UserRoleCount urc = new UserRoleCount();
                urc.Role = role;
                urc.Count = count;
                urcList.Add(urc);
            }
            return urcList;
        }
    }
}
