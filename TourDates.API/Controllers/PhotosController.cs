using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using TourDates.API.Data;
using TourDates.API.Dtos;
using TourDates.API.Helpers;
using TourDates.API.Models;

namespace TourDates.API.Controllers
{
    [Authorize]
    [Route("api/users/{userId}/photos")]
    [ApiController]
    public class PhotosController : ControllerBase
    {
        private readonly CloudinarySettings _cloudinarySettings;
        private readonly IUserRepository _userRepository;
        private Cloudinary _cloudinary;
        private readonly IMapper _mapper;

        public PhotosController(
            IOptions<CloudinarySettings> cloudinarySettings,
            IMapper mapper,
            IUserRepository userRepository)
        {
            this._mapper = mapper;
            this._cloudinarySettings = cloudinarySettings.Value;
            this._userRepository = userRepository;

                        Account acc = new Account(
                _cloudinarySettings.CloudName,
                _cloudinarySettings.ApiKey,
                _cloudinarySettings.ApiSecret
            );

            _cloudinary = new Cloudinary(acc);
        }

        [HttpGet("{id}", Name = "GetPhoto")]
        public async Task<IActionResult> GetPhoto(int id)
        {
            var photoFromRepo = await _userRepository.GetPhoto(id);
            var photo = _mapper.Map<PhotoForReturnDto>(photoFromRepo);

            return this.Ok(photo);
        }

        [HttpPost]
        public async Task<IActionResult> AddPhotoForUser(int userId, [FromForm] PhotoForCreationDto photoForCreationDto)
        {
            if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
            {
                return this.Unauthorized();
            }
            var user = await _userRepository.GetUser(userId);

            var file = photoForCreationDto.File;
            var uploadResult = new ImageUploadResult();

            if (file.Length > 0) 
            {
                using (var stream = file.OpenReadStream())
                {
                    var uploadParams = new ImageUploadParams()
                    {
                        File = new FileDescription(file.Name, stream),
                        Transformation = new Transformation()
                            .Width(500).Height(500).Crop("fill").Gravity("face")
                    };

                    uploadResult = _cloudinary.Upload(uploadParams);
                }
            }

            photoForCreationDto.Url = uploadResult.Uri.ToString();
            photoForCreationDto.PublicId = uploadResult.PublicId;

            var photo = _mapper.Map<Photo>(photoForCreationDto);

            if (!user.Photos.Any(u => u.IsMain)) 
            {
                photo.IsMain = true;                
            }

            user.Photos.Add(photo);

            if (await this._userRepository.SaveAll())
            {
                var photoForReturnDto = _mapper.Map<PhotoForReturnDto>(photo);
                return this.CreatedAtRoute("GetPhoto", new { id = photo.Id }, photoForReturnDto);
            }

            return this.BadRequest("Could not add the photo");
        }

        [HttpPost("{id}/setMain")]
        public async Task<IActionResult> SetMainPhoto(int userId, int id)
        {
            if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
            {
                return this.Unauthorized();
            }

            var user = await _userRepository.GetUser(userId);
            
            if (!user.Photos.Any(x => x.Id == id))
            {
                return this.Unauthorized();
            }

            var photo = await _userRepository.GetPhoto(id);

            if (photo.IsMain)
            {
                return this.BadRequest("This photo is already the main photo");
            }

            var currentMainPhoto = await _userRepository.GetMainPhotoForUser(userId);        
            currentMainPhoto.IsMain = false;  
            photo.IsMain = true;

            if(await this._userRepository.SaveAll())
            {
                return this.NoContent();
            }

            return this.BadRequest("Could not set this photo to be main");
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePhoto(int userId, int id)
        {
            #region "Validation"

            if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
            {
                return this.Unauthorized();
            }

            var user = await _userRepository.GetUser(userId);
            
            if (!user.Photos.Any(x => x.Id == id))
            {
                return this.Unauthorized();
            }

            var photo = await _userRepository.GetPhoto(id);

            if (photo.IsMain)
            {
                return this.BadRequest("You cannot delete the main photo");
            }
            #endregion

            if (!string.IsNullOrWhiteSpace(photo.PublicId))
            {
                var result = _cloudinary.Destroy(new DeletionParams(photo.PublicId));

                if (string.Equals(result.Result, "ok", StringComparison.OrdinalIgnoreCase))
                {
                    _userRepository.Delete(photo);            
                }
            }
            else
            {
                _userRepository.Delete(photo);  
            }          

            if (await _userRepository.SaveAll())
            {
                return Ok();
            }

            return BadRequest("Failed to delete photo");
        }
    }
}