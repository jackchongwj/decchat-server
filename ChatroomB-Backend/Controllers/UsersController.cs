using Microsoft.AspNetCore.Mvc;
using ChatroomB_Backend.Models;
using ChatroomB_Backend.Service;
using ChatroomB_Backend.DTO;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using ChatroomB_Backend.Utils;
using Microsoft.IdentityModel.Tokens;

namespace ChatroomB_Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _UserService;
        private readonly IAuthUtils _authUtils;

        public UsersController(IUserService service, IAuthUtils authUtils)
        {
            _UserService = service;
            _authUtils = authUtils;
        }

        [HttpGet("Search")]
        [Authorize]
        public async Task<IActionResult> SearchByProfileName(string profileName)
        {
            try
            {
                int userId = _authUtils.ExtractUserIdFromJWT(HttpContext.User);

            if (!profileName.IsNullOrEmpty())
            {
                IEnumerable<UserSearchDetails> GetUserByName = await _UserService.GetByName(profileName.Trim(), userId);

                return Ok(GetUserByName);
            }

                return BadRequest(new { ErrorMessage = "Profile Name Cannot Be Empty" });
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

        }


        [HttpGet("RetrieveChatListByUser")]
        [Authorize]
        public async Task<IActionResult> GetChatListByUserId()
        {
            try
            {
                int userIdResult = _authUtils.ExtractUserIdFromJWT(HttpContext.User);

                IEnumerable<ChatlistVM> chatList = await _UserService.GetChatListByUserId(userIdResult);

                return Ok(chatList);
            }
            catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        [HttpGet("FriendRequest")]
        [Authorize]
        public async Task<IActionResult> GetFriendRequest()
        {
            try
            {
                int userId = _authUtils.ExtractUserIdFromJWT(HttpContext.User);

                IEnumerable<Users> GetFriendRequest = await _UserService.GetFriendRequest(userId);

                return Ok(GetFriendRequest);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            
        }

        [HttpGet("UserDetails")]
        [Authorize]
        public async Task<ActionResult<Users>> GetUserById()
        {
                int userId = _authUtils.ExtractUserIdFromJWT(HttpContext.User);

                Users user = await _UserService.GetUserById(userId);

                if (user == null)
                {
                    return NotFound("User ID not found");
                }

                return Ok(user);
        }

        [HttpPost("UpdateProfileName")]
        [Authorize]
        public async Task<IActionResult> UpdateProfileName([FromBody] UpdateProfileName model)
        {
            int userId = _authUtils.ExtractUserIdFromJWT(HttpContext.User);

            if (model == null || !ModelState.IsValid)
                {
                    return BadRequest("Invalid request data");
                }
 
            int result = await _UserService.UpdateProfileName(userId, model.NewProfileName);

            if (result == 0) return NotFound("User ID not found or update failed.");

            return Ok();
                
        }

        [HttpPost("UpdateProfilePicture")]
        [Authorize]
        public async Task<IActionResult> UpdateProfilePicture([FromForm] IFormFile file)
        {
            int userId = _authUtils.ExtractUserIdFromJWT(HttpContext.User);

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

        [HttpPost("UserDeletion")]
        [Authorize]
        public async Task<IActionResult> DeleteUser()
        {
            int userId = _authUtils.ExtractUserIdFromJWT(HttpContext.User);
         
            int result = await _UserService.DeleteUser(userId);

            if (result == 0) 
            { 
                return NotFound("User ID not found."); 
            }
            else 
            { 
                return Ok(); 
            }
            
        }

    }
}
