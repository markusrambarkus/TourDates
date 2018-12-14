using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TourDates.API.Data;
using TourDates.API.Dtos;
using TourDates.API.Models;

namespace TourDates.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        public UsersController(
            IMapper mapper,
            IUserRepository userRepository)
        {
            this._mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            this._userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        }

        [HttpGet]
        public async Task<IActionResult> GetUsers()
        {
            var users = await _userRepository.GetUsers();
            var usersToReturn = this._mapper.Map<IEnumerable<UserForListDto>>(users);

            return this.Ok(usersToReturn);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetUser(int id)
        {
            var user = await _userRepository.GetUser(id);
            var userToReturn = this._mapper.Map<UserForDetailDto>(user);

            return this.Ok(userToReturn);
        }
    }
}