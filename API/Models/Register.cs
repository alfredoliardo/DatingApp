using System.ComponentModel.DataAnnotations;

namespace API.Models
{
    public class Register
    {
        [Required]
        public string UserName { get; set; }
        [Required]
        [StringLength(16, MinimumLength = 4)]
        [DataType(DataType.Password)]
        public string Password {get;set;}
    }
}