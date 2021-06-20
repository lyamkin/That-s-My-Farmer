using System;
using System.ComponentModel.DataAnnotations;

namespace MyFarmer.Models
{
    public class Food
    {
        public int Id { get; set; }

        [Required, MinLength(1), MaxLength(255)]
        public string Description { get; set; }

    }
}