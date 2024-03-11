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
                ActionResult<int> userIdResult = _authUtils.ExtractUserIdFromJWT(HttpContext.User);
                if (userIdResult.Result is not null)
                {
                    // If there is an ActionResult, it means there was an error, return it
                    return userIdResult.Result;
                }

                if (!profileName.IsNullOrEmpty())
                {
                    IEnumerable<UserSearchDetails> GetUserByName = await _UserService.GetByName(profileName.Trim(), userIdResult.Value);

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
                ActionResult<int> userIdResult = _authUtils.ExtractUserIdFromJWT(HttpContext.User);
                if (userIdResult.Result is not null)
                {
                    // If there is an ActionResult, it means there was an error, return it
                    return userIdResult.Result;
                }

                IEnumerable<ChatlistVM> chatList = await _UserService.GetChatListByUserId(userIdResult.Value);

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
                ActionResult<int> userIdResult = _authUtils.ExtractUserIdFromJWT(HttpContext.User);
                if (userIdResult.Result is not null)
                {
                    // If there is an ActionResult, it means there was an error, return it
                    return userIdResult.Result;
                }

                IEnumerable<Users> GetFriendRequest = await _UserService.GetFriendRequest(userIdResult.Value);

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
                ActionResult<int> userIdResult = _authUtils.ExtractUserIdFromJWT(HttpContext.User);
                if (userIdResult.Result is not null)
                {
                    // If there is an ActionResult, it means there was an error, return it
                    return userIdResult.Result;
                }

                Users user = await _UserService.GetUserById(userIdResult.Value);

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
                ActionResult<int> userIdResult = _authUtils.ExtractUserIdFromJWT(HttpContext.User);
                if (userIdResult.Result is not null)
                {
                    // If there is an ActionResult, it means there was an error, return it
                    return userIdResult.Result;
                }

                if (model == null || !ModelState.IsValid)
                {
                    return BadRequest("Invalid request data");
                }

                model.Id = userIdResult.Value;

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
                ActionResult<int> userIdResult = _authUtils.ExtractUserIdFromJWT(HttpContext.User);
                if (userIdResult.Result is not null)
                {
                    // If there is an ActionResult, it means there was an error, return it
                    return userIdResult.Result;
                }

                byte[] filebyte = await ConvertToByteArrayAsync(file);

                if (file == null || file.Length == 0)
                {
                    return BadRequest("File is not provided or empty.");
                }

                int success = await _UserService.UpdateProfilePicture(userIdResult.Value, filebyte, file.FileName);

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
                ActionResult<int> userIdResult = _authUtils.ExtractUserIdFromJWT(HttpContext.User);
                if (userIdResult.Result is not null)
                {
                    // If there is an ActionResult, it means there was an error, return it
                    return userIdResult.Result;
                }
                int result = await _UserService.DeleteUser(userIdResult.Value);

                if (result == 0) { return NotFound("User ID not found."); }
                else { return Ok(); }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
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
