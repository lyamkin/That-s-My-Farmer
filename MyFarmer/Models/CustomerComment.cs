using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace MyFarmer.Models
{
    public class CustomerComment
    {
        public int Id { get; set; }

        [Required]
        public IdentityUser Customer { get; set; }

        [Required]
        public Farm Farm { get; set; }

        [Required]
        public DateTime Created { get; set; }

        [Required, MinLength(1), MaxLength(2000)]
        public string Message { get; set; }

        public int Rating { get; set; }
    }
}