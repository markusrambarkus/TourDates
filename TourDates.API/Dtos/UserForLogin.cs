using System.ComponentModel.DataAnnotations;

namespace TourDates.API.Dtos
{
    public class UserForLogin
    {
        public string Username { get; set; }
        
        public string Password { get; set; } 
    }
}