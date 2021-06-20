using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MyFarmer.Models;
using MyFarmer.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace MyFarmer
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddRazorPages();
            services.AddDbContext<MyFarmerDbContext>();

            services.AddIdentity<IdentityUser, IdentityRole>(options => 
            {
                // Password settings.
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = true;
                options.Password.RequiredLength = 6;
                options.Password.RequiredUniqueChars = 3;

                // Lockout settings.
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
                options.Lockout.MaxFailedAccessAttempts = 5;
                options.Lockout.AllowedForNewUsers = true;

                // User settings.
                options.User.AllowedUserNameCharacters =
                    "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
                options.User.RequireUniqueEmail = true;
                // allow users with EmailConfirmed value 0/false to log in
                options.SignIn.RequireConfirmedAccount = false;})
      .AddEntityFrameworkStores < MyFarmerDbContext>();
            services.ConfigureApplicationCookie(options =>
            {
                // Cookie settings
                options.Cookie.HttpOnly = true;
                options.ExpireTimeSpan = TimeSpan.FromMinutes(250);

                options.LoginPath = "/Account/Login";
                options.AccessDeniedPath = "/AccessDenied";
                options.SlidingExpiration = true;
        
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();

            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseAuthentication();

            app.UseRouting();

            app.UseAuthorization();
            SeedUsersAndRoles(userManager, roleManager);

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
            });
        }
         private void SeedUsersAndRoles(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager) {
            // create array with roles
            string[] roleNamesList = new string[] { "User", "Admin","Farmer" };

            foreach (string roleName in roleNamesList)
            {
                // check if role not exist
                if (!roleManager.RoleExistsAsync(roleName).Result) {
                    // create role object
                    IdentityRole role = new IdentityRole();
                    // add new role
                    role.Name = roleName;
                    // save new role in the db
                    IdentityResult result = roleManager.CreateAsync(role).Result;
                }
            }

            // WARNING: For testing ONLY. Do NOT do it on a production system!
            // Create an Administrator. 
            string adminEmail = "admin@admin.com";
            string adminPass = "Admin123!"; // terrible password
            if (userManager.FindByNameAsync(adminEmail).Result == null)
            {
                IdentityUser user = new IdentityUser();
                user.UserName = adminEmail;
                user.Email = adminEmail;
                user.EmailConfirmed = true;
                IdentityResult result = userManager.CreateAsync(user, adminPass).Result;
                if (result.Succeeded)
                {
                    IdentityResult result2 = userManager.AddToRoleAsync(user, "Admin").Result;
                    if (!result2.Succeeded) {
                        // FIXME: log the error
                    }
                } else {
                    // FIXME: log the error
                }
            }
        }
    }
}
