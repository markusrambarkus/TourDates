using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using TourDates.API.Data;
using TourDates.API.Dtos;
using TourDates.API.Models;

namespace TourDates.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthRepository _authRepository;
        private readonly IConfiguration _configuration;

        public AuthController(IAuthRepository authRepository, IConfiguration configuration)
        {
            this._authRepository = authRepository ?? throw new ArgumentNullException();
            this._configuration = configuration;
        }

        [HttpPost("register")]
        public async Task<IActionResult> RegisterAsync(UserForRegister userForRegisterDto)
        {
            userForRegisterDto.Username = userForRegisterDto.Username.ToLower();

            if (await _authRepository.UserExistsAsync(userForRegisterDto.Username))
            {
                return this.BadRequest("Username already exists");
            }

            var userToCreate = new User()
            {
                Username = userForRegisterDto.Username,
            };

            var createdUser = await _authRepository.RegisterAsync(userToCreate, userForRegisterDto.Password);

            return this.StatusCode(201);
        }

        [HttpPost("login")]
        public async Task<IActionResult> LoginAsync(UserForLogin userForLoginDto)
        {
            var user = await _authRepository.LoginAsync(userForLoginDto.Username, userForLoginDto.Password);

            if (user == null)
            {
                return this.Unauthorized();
            }

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Username)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration.GetSection("AppSettings:Token").Value));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);
            var tokenDescriptor = new SecurityTokenDescriptor()
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(1),
                SigningCredentials = credentials
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);

            return this.Ok(new {
                token = tokenHandler.WriteToken(token)
            });
        }
    }
}