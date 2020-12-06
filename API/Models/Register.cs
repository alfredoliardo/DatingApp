using System;
using System.ComponentModel.DataAnnotations;

namespace API.Models
{
    public class Register
    {
        [Required]
        public string UserName { get; set; }
        [Required]
        public string KnownAs { get; set; }
        [Required]
        public string Gender { get; set; }
        [Required]
        public DateTime DateOfBirth { get; set; }
                [Required]
        public string City { get; set; }
                [Required]
        public string Country { get; set; }
        [Required]
        [StringLength(16, MinimumLength = 4)]
        [DataType(DataType.Password)]
        public string Password {get;set;}
    }
}