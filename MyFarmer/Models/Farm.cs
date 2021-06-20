using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace MyFarmer.Models
{
    public class Farm
    {
        public int Id { get; set; }
        [Required]
        public IdentityUser Farmer { get; set; }

        [Required, MinLength(1), MaxLength(255)]
        public string Name { get; set; }

        [Required, EmailAddress]
        public string Email { get; set; }

        [Required, Phone]
        public string Phone { get; set; }

        [Required, MinLength(1), MaxLength(500)]
        public string Address { get; set; }

        [MinLength(1), MaxLength(500)]
        public string Description { get; set; }
    }
}