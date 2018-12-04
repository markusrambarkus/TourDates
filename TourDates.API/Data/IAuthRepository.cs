using System.Threading.Tasks;
using TourDates.API.Models;

namespace TourDates.API.Data
{
    public interface IAuthRepository
    {
         Task<User> RegisterAsync(User user, string password);
         Task<User> Login(string username, string password);
         Task<bool> UserExists(string username);
    }
}