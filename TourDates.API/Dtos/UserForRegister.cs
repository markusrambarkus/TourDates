using System.ComponentModel.DataAnnotations;

namespace TourDates.API.Dtos
{
    public class UserForRegister
    {
        [Required]
        public string Username { get; set; }
        
        [Required]
        [StringLength(8, MinimumLength = 8, ErrorMessage="Minimum password length is 8")]
        public string Password { get; set; }
    }
}