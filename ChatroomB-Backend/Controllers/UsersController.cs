using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ChatroomB_Backend.Data;
using ChatroomB_Backend.Models;
using ChatroomB_Backend.Service;
using Microsoft.AspNetCore.SignalR;
using Microsoft.CodeAnalysis.Elfie.Diagnostics;
using ChatroomB_Backend.DTO;
using System.Linq;

namespace ChatroomB_Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _UserService;


        public UsersController(IUserService service)
        {
            _UserService = service;
        }

        [HttpGet("Search")]
        public async Task<IActionResult> SearchByProfileName(string profileName, int userId)
        {
            IEnumerable<UserSearch> GetUserByName = await _UserService.GetByName(profileName, userId);

            //await _hubContext.Clients.All.SendAsync("ReceiveSearchResults", GetUserByName);

            return Ok(GetUserByName);
        }

        [HttpGet("GetChatListByUserId")]
        public async Task<IActionResult> GetChatListByUserId(int userId)
        {
            var friendList = await _UserService.GetChatListByUserId(userId);
            return Ok(friendList); //HTTP 200 OK indicates that the request was successful, and the server is returning the requested data.
        }

        [HttpGet("FriendRequest")]
        public async Task<IActionResult> GetFriendRequest(int userId)
        {
            IEnumerable<Users> GetFriendRequest = await _UserService.GetFriendRequest(userId);

            return Ok(GetFriendRequest);
        }

        [HttpGet("UserDetails")]
        public async Task<ActionResult<Users>> GetUserById(int id)
        {
            var user = await _UserService.GetUserById(id);
            if (user == null)
            {
                return NotFound();
            }

            return Ok(user);
        }

        [HttpPut("UpdateProfileName")]
        public async Task<IActionResult> UpdateProfileName(int id, [FromBody] string newProfileName)
        {

            var result = await _UserService.UpdateProfileName(id,newProfileName);
            if (result == 0) return NotFound();

            return Ok();
        }

        [HttpPut("UpdateProfilePicture")]
        public async Task<IActionResult> UpdateProfilePicture(int id, IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("File is not provided or empty.");
            }

            var user = await _UserService.GetUserById(id);
            if (user == null) return NotFound("User not found.");

            try
            {
                // Assuming BlobService.UploadImageFiles returns the URI of the uploaded image
                var fileUri = await _UserService.UploadProfilePicture(file, id);

                var result = await _UserService.UpdateProfilePicture(id, fileUri);
                if (result == 0) return NotFound("Failed to update the profile picture.");

                return Ok(new { Message = "Profile picture updated successfully.", Uri = fileUri });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }


        [HttpDelete("UserDeletion")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            int result = await _UserService.DeleteUser(id);
            if (result == 0) { return BadRequest(); }
            else { return Ok(); }

        }               
    }
}
