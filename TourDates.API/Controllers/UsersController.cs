using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TourDates.API.Data;
using TourDates.API.Models;

namespace TourDates.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        public UsersController(IMapper mapper, IUserRepository userRepository)
        {
            this._userRepository = userRepository ?? throw new ArgumentNullException();
        }

        [HttpGet]
        [Produces(typeof(IEnumerable<User>))]
        public async Task<IActionResult> GetUsers()
        {
            var users = await _userRepository.GetUsers();
            return this.Ok(users);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetUser(int id)
        {
            var user = await _userRepository.GetUser(id);
            return this.Ok(user);
        }
    }
}