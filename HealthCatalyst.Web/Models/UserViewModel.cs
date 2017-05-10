using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace HealthCatalyst.Web.Models
{
    public class UserViewModel
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string FirstName { get; set; }

        [Required]
        [StringLength(100)]
        public string LastName { get; set; }

        [StringLength(200)]
        public string Address { get; set; }

        [Required]
        [RegularExpression(@"^\d*$", ErrorMessage = "Please enter valid Age.")]
        public int Age { get; set; }

        [StringLength(200)]
        public string Interests { get; set; }

        [StringLength(200)]
        public string PictureFile { get; set; }
    }
}