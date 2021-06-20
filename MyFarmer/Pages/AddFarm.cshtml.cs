using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MyFarmer.Data;
using MyFarmer.Models;

namespace MyFriends2.Pages
{
    // TODO: Only Admin and farmers allowed
    [Authorize(Roles = "Farmer")]

    public class AddFarmModel : PageModel
    {
        private readonly MyFarmerDbContext db;
        private readonly ILogger<AddFarmModel> logger;
        private readonly SignInManager<IdentityUser> signInManager;
        private readonly UserManager<IdentityUser> userManager;
        private readonly IWebHostEnvironment webHostEnvironment;

        public AddFarmModel(MyFarmerDbContext db, ILogger<AddFarmModel> logger,SignInManager<IdentityUser> signInManager, UserManager<IdentityUser> userManager,IWebHostEnvironment webHostEnvironment)
        {
            this.db = db;
            this.logger = logger;
            this.signInManager = signInManager;
            this.userManager = userManager;
            this.webHostEnvironment = webHostEnvironment;
        }
        // [BindProperty]
        // public Farm NewFarm {get; set;} = new Farm();
        [BindProperty]
        public InputModelFarm InputsFarm { get; set; }
        public class InputModelFarm
        {
            [Required]
            [Display(Name = "Farm Name")]
            public string Name { get; set; }

            [Required]
            [EmailAddress]
            [Display(Name = "Email")]
            public string Email { get; set; }

            [Required]
            [DataType(DataType.PhoneNumber)]
            [Display(Name = "Phone")]
            public string Phone { get; set; }

            [Required]
            [Display(Name = "Farm Address")]
            public string Address { get; set; }

            [Required]
            [Display(Name = "Farm Description")]
            public string Description { get; set; }
        }
        [BindProperty]
        public List<InputModelFarmFood> InputsFarmFoodList { get; set; }
        public class InputModelFarmFood
        {
            [Required]
            public int Id { get; set; }
            [Required, MinLength(1), MaxLength(255)]
            public string Description { get; set; }
            public bool IsChecked { get; set; }
        }  
        [BindProperty]
        public List<InputModelFarmServices> InputsFarmServicesList { get; set; }
        public class InputModelFarmServices
        {
            [Required]
            public int Id { get; set; }
            [Required, MinLength(1), MaxLength(255)]
            public string Description { get; set; }
            public bool IsChecked { get; set; }
        }  
         [BindProperty]
        public IFormFile Upload { get; set; }
        public List<Food> FoodList {get; set;} // For add farm
        public List<Service> ServicesList { get; set; } // for add farm
        public List<FarmsFood> FarmsFoodList {get; set;} // for update farm
        public List<FarmsService> FarmsServicesList {get; set;} // for update farm
        public List<FarmImage> ImagesList {get; set;}
        [BindProperty(SupportsGet =true)]
        public int? Id { get; set; }
    

        // 3. Create and save the new farm
        public async Task<IActionResult> OnGetAsync()
        {
            // Get the user
            var userName = User.Identity.Name;
            var user = await userManager.FindByNameAsync(userName);
            // Save email into NewFarm. For display purposes email in the form.
            InputsFarm = new InputModelFarm();
            InputsFarmFoodList = new List<InputModelFarmFood>();
            InputsFarmServicesList = new List<InputModelFarmServices>();
            // Check if UPDATE the current farm. Id expected
            if(Id != null)
            {
                // get farm to Update By Id
                var myFarm = db.Farms.FirstOrDefault(farm => farm.Id==Id);
                if(myFarm == null)
                {
                    logger.LogError($"Internal error: farm with id{Id} not found");
                    return RedirectToPage("/NotFound");
                }
                // If farm fetched from the db
                InputsFarm.Name = myFarm.Name;
                InputsFarm.Email = myFarm.Email;
                InputsFarm.Phone = myFarm.Phone;
                InputsFarm.Address = myFarm.Address;
                InputsFarm.Description = myFarm.Description;
                
                // Fetch data for food services
                FarmsFoodList = new List<FarmsFood>();
                FarmsFoodList = db.FarmsFoods.Include(farm => farm.Farm).Include(farm => farm.FarmFood).Where(farm => farm.Farm.Id == Id).ToList();
                FarmsFoodList.ForEach(food => {
                InputModelFarmFood InputsFarmFood = new InputModelFarmFood();
                InputsFarmFood.Id = food.FarmFood.Id;
                InputsFarmFood.Description = food.FarmFood.Description;
                InputsFarmFood.IsChecked = food.IsChecked;
                InputsFarmFoodList.Add(InputsFarmFood);
            });
                // Fetch data for services
                FarmsServicesList = new List<FarmsService>();
                FarmsServicesList = db.FarmsServices.Include(farm => farm.Farm).Include(farm => farm.FarmService).Where(farm => farm.Farm.Id == Id).ToList();
                FarmsServicesList.ForEach(service => {
                    InputModelFarmServices InputsFarmService = new InputModelFarmServices();
                    InputsFarmService.Id = service.FarmService.Id;
                    InputsFarmService.Description = service.FarmService.Description;
                    InputsFarmService.IsChecked = service.IsChecked;
                    InputsFarmServicesList.Add(InputsFarmService);
                });

                return Page();
            }

            // CODE BELOW FOR ADD NEW FARM
            InputsFarm.Email = user.Email;
            // Get food list options and create list of food options to bind with form
            FoodList = new List<Food>();
            FoodList = db.Foods.ToList();
            FoodList.ForEach(food => {
                InputModelFarmFood InputsFarmFood = new InputModelFarmFood();
                InputsFarmFood.Id = food.Id;
                InputsFarmFood.Description = food.Description;
                InputsFarmFood.IsChecked = false;
                InputsFarmFoodList.Add(InputsFarmFood);
            });
            // Get services list options and create list of services options to bind with form
            ServicesList = new List<Service>();
            ServicesList = db.Servises.ToList();
            ServicesList.ForEach(service => {
                InputModelFarmServices InputsFarmService = new InputModelFarmServices();
                InputsFarmService.Id = service.Id;
                InputsFarmService.Description = service.Description;
                InputsFarmService.IsChecked = false;
                InputsFarmServicesList.Add(InputsFarmService);
            });
            // Get images list
            //ImagesList = db.FarmImages.ToList();
            return Page();
        }
        public async Task<IActionResult> OnPostAsync()
        {
            // 1. Validation inputs
            if(!ModelState.IsValid) 
            {
               return Page();
            }

            // CODE BELOW TO UPDATE EXISTING FARM BASED ON FARM ID
            if(Id != null)
            {
                // Fetch existing Farm by Id and update fields
                Farm updatedFarm = db.Farms.FirstOrDefault(farm=>farm.Id == Id);
                updatedFarm.Name = InputsFarm.Name;
                updatedFarm.Email = InputsFarm.Email;
                updatedFarm.Phone = InputsFarm.Phone;
                updatedFarm.Address = InputsFarm.Address;
                updatedFarm.Description = InputsFarm.Description;
                db.Farms.Update(updatedFarm);
                logger.LogInformation($"Farm {InputsFarm.Name} was update");
                // Fetch existing Food services and update status for each food (checked or not)
                    // Loop over FoodList and save records in the FarmsFood
                InputsFarmFoodList.ForEach(item => {
                    FarmsFood updatedFarmsFood = db.FarmsFoods.Include(farm=>farm.Farm).Include(farm=>farm.FarmFood).Where(farm=> farm.Farm.Id == Id && farm.FarmFood.Id == item.Id).FirstOrDefault();
                    updatedFarmsFood.IsChecked = item.IsChecked;
                    db.FarmsFoods.Update(updatedFarmsFood);
                    logger.LogInformation($"The food: {item.Description} was updated in the farm {updatedFarm.Name}");
                });
                // Fetch existing Services and update status for each service (checked or not)
                    // Loop over Services list and save records in the FarmsServices
                InputsFarmServicesList.ForEach(item => {
                    FarmsService updatedFarmsService = db.FarmsServices.Include(farm=>farm.Farm).Include(farm=>farm.FarmService).Where(farm=> farm.Farm.Id == Id && farm.FarmService.Id == item.Id).FirstOrDefault();
                    updatedFarmsService.IsChecked = item.IsChecked;
                    db.FarmsServices.Update(updatedFarmsService);
                    logger.LogInformation($"The service: {item.Description} was updated in the farm {updatedFarm.Name}");
                });

                // Upload new images
                if(Upload !=null) {
                    string uniqueFileName = handleImageSaving();
                    if(uniqueFileName == null) {
                        return Page();
                    }
                    // Fetch existing image with farm by farm Id
                    FarmImage updatedFarmImage = db.FarmImages.Include(farm=>farm.Farm).FirstOrDefault(farm=>farm.Farm.Id==Id);
                    updatedFarmImage.ImageName = uniqueFileName;
                    db.FarmImages.Update(updatedFarmImage);
                    logger.LogInformation($"The image: {uniqueFileName} was added to the farm {updatedFarm.Name}");
                }
                db.SaveChanges();
                TempData["message"] = $"Farm {InputsFarm.Name} was updated";
                return RedirectToPage("/FarmsList");
            }

            // CODE BELOW TO ADD A NEW FARM
            // 2. Create and save a new Farm
                // Get the user
            var user = await userManager.FindByNameAsync(User.Identity.Name);
                // Create a new farm
            Farm NewFarm = new Farm {Farmer = user, Name = InputsFarm.Name, Email = InputsFarm.Email, Phone = InputsFarm.Phone, Address = InputsFarm.Address, Description = InputsFarm.Description};
                // Add new farm into the database
            db.Farms.Add(NewFarm); 
                // Try to save changes
            await db.SaveChangesAsync();
                // Get a Farm Id
            int? FarmId = NewFarm.Id;
            // 3. Add food and services
                // Check if id not null
            if(FarmId != null)
            {
                try {
                    // Loop over FoodList and save records in the FarmsFood
                    InputsFarmFoodList.ForEach(item => {
                        Food foodItem = db.Foods.FirstOrDefault(i => i.Id == item.Id); // Get food object from db
                        db.FarmsFoods.Add(new FarmsFood{FarmFood = foodItem, Farm = NewFarm, IsChecked = item.IsChecked}); // Create new record in FarmsFoods table
                        logger.LogInformation($"New food: {item.Description} was added to farm {NewFarm.Name}");
                    });
                    // Loop over Services list and save records in the FarmsServices
                    InputsFarmServicesList.ForEach(item => {
                        Service serviceItem = db.Servises.FirstOrDefault(i => i.Id == item.Id); // Get services object from db
                        db.FarmsServices.Add(new FarmsService{FarmService = serviceItem, Farm = NewFarm, IsChecked = item.IsChecked}); // Create new record in FarmsFoods table
                        logger.LogInformation($"New service: {item.Description} was added to farm {NewFarm.Name}");
                    });
                    // Upload images
                    
                    if(Upload !=null) {
                        string uniqueFileName = handleImageSaving();
                        if(uniqueFileName == null) {
                            throw new Exception();
                        }
                        FarmImage newFarmImage = new FarmImage {Farm = NewFarm, ImageName = uniqueFileName};
                        db.FarmImages.Add(newFarmImage);
                        logger.LogInformation($"New image: {uniqueFileName} was added to the farm {NewFarm.Name}");
                    }
                    await db.SaveChangesAsync();
                    // If any errors than delete farm created before
                } catch(Exception e) {
                    logger.LogError($"Error: adding new food, image or services to the farm for user {user.Email}: {e.Message}");
                    // remove farm created before
                    db.Farms.Remove(NewFarm);
                    await db.SaveChangesAsync();
                    ModelState.AddModelError(string.Empty, $"Error: adding new food, image or services to the farm for user {user.Email}");
                    return Page();
                }
                // If no id after saving the new farm than return error
            } else {
                logger.LogError($"Error adding a new farm for user {user.Email}");
                ModelState.AddModelError(string.Empty,$"Error adding a new farm for user {user.Email}");
                return Page();
            }
            TempData["message"] = $"The new Farm {NewFarm.Name} was created"; // save message in temporary storage
            logger.LogInformation($"The new Farm {NewFarm.Name} was created");
            return RedirectToPage("/FarmsList"); 
        }
        // Function for saving image on the server and return file name. If any error than return null
        private string handleImageSaving()
        {
            string uniqueFileName = null;
            if(Upload !=null) {
                string fileExtension = Path.GetExtension(Upload.FileName).ToLower(); // get file extension
                string[] allowedExtensions = {".jpg",".jpeg",".gif",".png"};
                // check if uploaded file is an image type
                if(!allowedExtensions.Contains(fileExtension)) {
                    ModelState.AddModelError(string.Empty, "Only image files (.jpg, .jpeg, .gif, .png) are allowed");
                    return null;
                } 
                // Physical Path to upload folder
                string uploadsFolder = Path.Combine(webHostEnvironment.WebRootPath, "Uploads");
                // Create unique file name
                uniqueFileName = Guid.NewGuid().ToString() + "_" + Upload.FileName;
                // Full path to file
                string destPath = Path.Combine(uploadsFolder, uniqueFileName);
                try {
                    using (var fileStream = new FileStream(destPath, FileMode.Create))
                    {
                        Upload.CopyTo(fileStream);
                    }
                } catch(Exception ex ) when (ex is IOException|| ex is SystemException) {
                    ModelState.AddModelError(string.Empty, "Internal error saving the uploaded image");
                    return null;
                }
            }
            return uniqueFileName;
        }

    }
}
