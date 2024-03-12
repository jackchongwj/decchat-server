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
            try
            {
                int userId = _authUtils.ExtractUserIdFromJWT(HttpContext.User);

                Users user = await _UserService.GetUserById(userId);

                if (user == null)
                {
                    return NotFound("User ID not found");
                }

                return Ok(user);
            }
            catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
            
        }
         

        [HttpPost("UpdateProfileName")]
        [Authorize]
        public async Task<IActionResult> UpdateProfileName([FromBody] UpdateProfileName model)
        {
            try
            {
                int userIdResult = _authUtils.ExtractUserIdFromJWT(HttpContext.User);

                if (model == null || !ModelState.IsValid)
                {
                    return BadRequest("Invalid request data");
                }

                model.Id = userIdResult;

                int result = await _UserService.UpdateProfileName(model.Id, model.NewProfileName);

                if (result == 0) return NotFound("User ID not found or update failed.");

                return Ok();

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

        }

        [HttpPost("UpdateProfilePicture")]
        [Authorize]
        public async Task<IActionResult> UpdateProfilePicture([FromForm] IFormFile file)
        {
            try
            {
                int userIdResult = _authUtils.ExtractUserIdFromJWT(HttpContext.User);

                byte[] filebyte = await ConvertToByteArrayAsync(file);

                if (file == null || file.Length == 0)
                {
                    return BadRequest("File is not provided or empty.");
                }

                int success = await _UserService.UpdateProfilePicture(userIdResult, filebyte, file.FileName);

                if (success == 0)
                {
                    return NotFound("Failed to update the profile picture.");
                }

                return Ok(new { Message = "Profile picture updated successfully." });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        [HttpPost("UserDeletion")]
        [Authorize]
        public async Task<IActionResult> DeleteUser()
        {
            try
            {
                int userIdResult = _authUtils.ExtractUserIdFromJWT(HttpContext.User);

                int result = await _UserService.DeleteUser(userIdResult);

                if (result == 0) { return NotFound("User ID not found."); }
                else { return Ok(); }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
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
