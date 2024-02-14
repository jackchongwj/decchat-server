using Microsoft.AspNetCore.Mvc;
using ChatroomB_Backend.Models;
using ChatroomB_Backend.Service;
using ChatroomB_Backend.DTO;

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
            try
            {
                //if (string.IsNullOrEmpty(profileName))
                //{
                //    return BadRequest("Profile name cannot be empty");
                //}
                //else if (userId == null) 
                //{
                //    return BadRequest("User ID cannot be null");
                //}
                //else
                //{
                //    IEnumerable<UserSearch> GetUserByName = await _UserService.GetByName(profileName, userId);

                //    return Ok(GetUserByName);
                //}

                IEnumerable<UserSearchDetails> GetUserByName = await _UserService.GetByName(profileName, userId);

                return Ok(GetUserByName);

            } catch (Exception ex) 
            {
                Console.Error.WriteLine($"Error in SearchByProfileName method: {ex.ToString()}");
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }


        [HttpGet("RetrieveChatListByUser")]
        public async Task<IActionResult> GetChatListByUserId([FromQuery] int userId)
        {
            var chatList = await _UserService.GetChatListByUserId(userId);
            return Ok(chatList); //HTTP 200 OK indicates that the request was successful, and the server is returning the requested data.
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

        [HttpPut("UserUpdate")]
        public async Task<IActionResult> UpdateUser(int id, [FromBody] Users user)
        {
            if (id != user.UserId)
            {
                return BadRequest();
            }

            int updatedUser = await _UserService.UpdateUser(user);
            if (updatedUser == 0)
            {
                return NotFound();
            }

            return Ok();
        }

        [HttpDelete("UserDeletion")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            int result = await _UserService.DeleteUser(id);
            if (result == 0) { return BadRequest(); }
            else { return Ok(); }

        }

        [HttpPost("UserDetails/PasswordChange")]
        public async Task<IActionResult> ChangePassword(int id, [FromBody] string newPassword)
        {
            await _UserService.ChangePassword(id, newPassword);
            return Ok();
        }

        [HttpGet("DoesUsernameExist")]
        public async Task<ActionResult> DoesUsernameExist(string username)
        {
            bool isUnique = await _UserService.DoesUsernameExist(username);

            return Ok(new { IsUnique = isUnique });
        }
      
        private async Task<byte[]> ConvertToByteArrayAsync(IFormFile file)
        {
            using (MemoryStream memoryStream = new MemoryStream())
            {
                await file.CopyToAsync(memoryStream);
                return memoryStream.ToArray();
            }
        }
        
    }
}
