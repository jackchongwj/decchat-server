using Microsoft.AspNetCore.Mvc;
using ChatroomB_Backend.Models;
using ChatroomB_Backend.Service;
using ChatroomB_Backend.DTO;
using Microsoft.AspNetCore.Authorization;

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
        [Authorize]
        public async Task<IActionResult> SearchByProfileName(string profileName, int userId)
        {
            try
            {
                IEnumerable<UserSearchDetails> GetUserByName = await _UserService.GetByName(profileName, userId);

                return Ok(GetUserByName);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }


        [HttpGet("RetrieveChatListByUser")]
        [Authorize]
        public async Task<IActionResult> GetChatListByUserId([FromQuery] int userId)
        {
            try
            {
                IEnumerable<ChatlistVM> chatList = await _UserService.GetChatListByUserId(userId);

                return Ok(chatList);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }

        }

        [HttpGet("FriendRequest")]
        [Authorize]
        public async Task<IActionResult> GetFriendRequest(int userId)
        {
            try
            {
                IEnumerable<Users> GetFriendRequest = await _UserService.GetFriendRequest(userId);

                return Ok(GetFriendRequest);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("UserDetails")]
        [Authorize]
        public async Task<ActionResult<Users>> GetUserById(int id)
        {
            try
            {
                Users user = await _UserService.GetUserById(id);

                if (user == null)
                {
                    return NotFound("User ID not found");
                }

                return Ok(user);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost("UpdateProfileName")]
        [Authorize]
        public async Task<IActionResult> UpdateProfileName([FromBody] UpdateProfileName model)
        {
            if (model == null || !ModelState.IsValid)
            {
                return BadRequest("Invalid request data");
            }
            
            try
            {
                int result = await _UserService.UpdateProfileName(model.Id, model.NewProfileName);

                if (result == 0) return NotFound("User ID not found or update failed.");

                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
            
        }

        [HttpPost("UpdateProfilePicture")]
        [Authorize]
        public async Task<IActionResult> UpdateProfilePicture([FromForm] IFormFile file, [FromForm(Name = "id")] string userId)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("File is not provided or empty.");
            }

            if (file.Length > 7 * 1024 * 1024)
            {
                return BadRequest("The file is too large. Please upload an image that is 7MB or smaller.");
            }

            byte[] fileBytes;
            using (MemoryStream memoryStream = new MemoryStream())
            {
                await file.CopyToAsync(memoryStream);
                fileBytes = memoryStream.ToArray();
            }

            try
            {
                int success = await _UserService.UpdateProfilePicture(Convert.ToInt32(userId), fileBytes, file.FileName);

                if (success == -1)
                {
                    return BadRequest("The uploaded file is not a valid image.");
                }
                else if (success == 0)
                {
                    return NotFound("Failed to update the profile picture.");
                }

                return Ok(new { Message = "Profile picture updated successfully." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost("UserDeletion")]
        [Authorize]
        public async Task<IActionResult> DeleteUser([FromQuery]int id)
        {
            try
            {
                int result = await _UserService.DeleteUser(id);
                if (result == 0) { return NotFound("User ID not found."); }
                else { return Ok(); }
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("DoesUsernameExist")]
        [AllowAnonymous]
        public async Task<IActionResult> DoesUsernameExist(string username)
        {
            bool isExist = await _UserService.DoesUsernameExist(username);

            return Ok(isExist);
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
