using System;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TourDates.API.Models;

namespace TourDates.API.Data
{
    public class AuthRepository : IAuthRepository
    {
        /// The data context
        private readonly DataContext _context;

        /// The default constructor
        public AuthRepository(DataContext context)
        {
            _context = context;
        }

        public async Task<User> LoginAsync(string username, string password)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => string.Equals(x.Username, username, StringComparison.OrdinalIgnoreCase));

            if (user == null)
            {
                return null;
            }

            if (!VerifyPasswordHash(password, user.PasswordHash, user.PasswordSalt))
            {
                return null;
            }

            return user;
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

        public async Task<bool> UserExistsAsync(string username)
        {
            return await _context.Users.AnyAsync(x => string.Equals(x.Username, username, StringComparison.OrdinalIgnoreCase));
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

        private bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            using(var hmac = new HMACSHA512(passwordSalt))
            {
                var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));

                return passwordHash.SequenceEqual(computedHash);
            }
        }

        #endregion
    }
}