using System;
using System.Security.Cryptography;
using System.Threading.Tasks;
using TourDates.API.Models;

namespace TourDates.API.Data
{
    public class AuthRepository : IAuthRepository
    {
        private readonly DataContext _context;
        public AuthRepository(DataContext context)
        {
            _context = context;
        }

        public Task<User> Login(string username, string password)
        {
            throw new System.NotImplementedException();
        }

        public async Task<User> RegisterAsync(User user, string password)
        {
            byte[] passwordHash, passwordSalt;
            CreatePasswordHash(password, out passwordHash, out passwordSalt);

            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;

            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            return user;
        }
        public Task<bool> UserExists(string username)
        {
            throw new System.NotImplementedException();
        }

        #region Private Methods

        private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using(var hmac = new HMACSHA512())
            {
                var passwordBytes = System.Text.Encoding.UTF8.GetBytes(password);

                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(passwordBytes);
            }
        }

        #endregion
    }
}