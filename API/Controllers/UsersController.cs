using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using API.Data;
using API.Entities;
using API.Extensions;
using API.Helpers;
using API.Interfaces;
using API.Models;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class UsersController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        private readonly IPhotoService _photoService;

        public UsersController(IUserRepository userRepository, IMapper mapper, IPhotoService photoService)
        {
            _photoService = photoService;
            _mapper = mapper;
            _userRepository = userRepository;
        }

        [HttpGet]

        public async Task<ActionResult<IEnumerable<Member>>> GetUsers([FromQuery]UserParams userParams)
        {
            var users = await _userRepository.GetMembersAsync(userParams);
            Response.AddPaginationHeader(users.CurrentPage, users.PageSize, users.TotalItems, users.TotalPages);
            return Ok(users);
        }


        //api/users/3
        [HttpGet("{username}", Name="GetUser")]
        public async Task<ActionResult<Member>> GetUser(string username)
        {
            var user = await _userRepository.GetMemberAsync(username);
            return Ok(user);
        }

        [HttpPut]
        public async Task<ActionResult> UpdateUser(Update user_updates)
        {
            var user = await _userRepository.GetUserByUserNameAsync(User.GetUsername());
            _mapper.Map(user_updates, user);
            _userRepository.Update(user);
            if (await _userRepository.SaveAllAsync()) return NoContent();
            return BadRequest("Failed to update user");
        }

        [HttpPost("add-photo")]
        public async Task<ActionResult<Models.Photo>> AddPhoto(IFormFile file)
        {
            var user = await _userRepository.GetUserByUserNameAsync(User.GetUsername());
            var result = await _photoService.UploadPhotoAsync(file);
            if(result.Error != null)return BadRequest(result.Error.Message);
            var photo = new Entities.Photo{
                Url = result.SecureUrl.AbsoluteUri,
                PublicId = result.PublicId
            };
            if(user.Photos.Count == 0){
                photo.IsMain = true;
            }
            user.Photos.Add(photo);
            if(await _userRepository.SaveAllAsync()){
                return CreatedAtRoute("GetUser",new{username = user.UserName},_mapper.Map<Models.Photo>(photo));
            }

            return BadRequest("Error during adding photo");
        }

        [HttpPut("set-main-photo/{photoId}")]
        public async Task<ActionResult> SetMainPhoto(int photoId){
            var user = await _userRepository.GetUserByUserNameAsync(User.GetUsername());

            var photo = user.Photos.Where(p => p.Id == photoId).SingleOrDefault();

            if(photo != null && photo.IsMain)
            return BadRequest("The selected photo already is the main photo.");

            var current = user.Photos.Where(p => p.IsMain).SingleOrDefault();
            if(current != null)current.IsMain = false;
            photo.IsMain = true;

            if(await _userRepository.SaveAllAsync())return NoContent();
            return BadRequest("Failed to set main photo");
        }

        [HttpDelete("delete-photo/{photoId}")]
        public async Task<ActionResult> DeletePhoto(int photoId){
            var user = await _userRepository.GetUserByUserNameAsync(User.GetUsername());

            var photo = user.Photos.Where(p => p.Id == photoId).SingleOrDefault();

            if(photo == null)return NotFound("Photo does not exitsts.");
            if(photo.IsMain)return BadRequest("You can't delete the main photo");

            if(photo.PublicId != null){
                var result = await _photoService.DeletePhotoAsync(photo.PublicId);
                if(result.Error != null) return BadRequest(result.Error.Message);
            }

            user.Photos.Remove(photo);
            
            if(await _userRepository.SaveAllAsync())return Ok();
            return BadRequest("Somthing was wrong deleting photo");
        }
    }
}