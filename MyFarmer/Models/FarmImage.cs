using System.ComponentModel.DataAnnotations;
using System;

namespace MyFarmer.Models
{
    public class FarmImage
    {
        public int Id { get; set; }

        [Required]
        public Farm Farm { get; set; }

        [Required]
        [MinLength(1), MaxLength(500)]
        public string ImageName { get; set; }
        public bool isChecked { get; set; }
    }
}