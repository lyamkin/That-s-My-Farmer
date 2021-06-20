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

namespace MyFarmer.Pages
{
    public class SearchResultModel : PageModel
    {
        [BindProperty(SupportsGet = true)]
        public string keywords { get; set; }
        private readonly MyFarmerDbContext db;

        private readonly RoleManager<IdentityRole> roleManager;
        public SearchResultModel(MyFarmerDbContext db)
        {
            this.db = db;
        }
        public List<Farm> FarmsList = new List<Farm>();
        public void OnGet()
        {
            var farms = db.Farms.ToList();

            if (String.IsNullOrEmpty(keywords))
            {
                FarmsList = farms;
            }
            else
            {
                FarmsList = farms.Where(f => f.Name.ToLower().Contains(keywords.ToLower()) || f.Address.ToLower().Contains(keywords.ToLower())).ToList();
            }
        
        }
    }
}
