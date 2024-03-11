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

            return BadRequest(new { ErrorMessage = "Cannot Empty" });
        }

        [HttpGet("RetrieveChatListByUser")]
        [Authorize]
        public async Task<IActionResult> GetChatListByUserId([FromQuery] int userId)
        {

            IEnumerable<ChatlistVM> chatList = await _UserService.GetChatListByUserId(userId);

            return Ok(chatList);
        }

        [HttpGet("FriendRequest")]
        [Authorize]
        public async Task<IActionResult> GetFriendRequest()
        {
            ClaimsIdentity? identity = HttpContext.User.Identity as ClaimsIdentity;
            if (identity == null)
            {
                return Unauthorized("User identity not found");
            }

            Claim userIdClaim = identity.FindFirst("userId")!;
            if (userIdClaim == null)
            {
                return Unauthorized("User ID claim not found in the token");
            }

            if (!int.TryParse(userIdClaim.Value, out int userId))
            {
                return BadRequest("Invalid User ID claim value");
            }

            IEnumerable<Users> GetFriendRequest = await _UserService.GetFriendRequest(userId);

            IEnumerable<Users> GetFriendRequest = await _UserService.GetFriendRequest(id);

            return Ok(GetFriendRequest);

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
            if (model == null || !ModelState.IsValid)
            {
                return BadRequest("Invalid request data");
            }
 
            int result = await _UserService.UpdateProfileName(model.Id, model.NewProfileName);

            if (result == 0) return NotFound("User ID not found or update failed.");

            return Ok(); 
        }

        [HttpPost("UpdateProfilePicture")]
        [Authorize]
        public async Task<IActionResult> UpdateProfilePicture([FromForm] IFormFile file, [FromForm(Name = "id")] string userId)
        {
            byte[] filebyte = await ConvertToByteArrayAsync(file);

            if (file == null || file.Length == 0)
            {
                return BadRequest("File is not provided or empty.");
            }

            int success = await _UserService.UpdateProfilePicture(Convert.ToInt32(userId), filebyte, file.FileName);

            if (success == 0)
            {
                return NotFound("Failed to update the profile picture.");
            }

            return Ok(new { Message = "Profile picture updated successfully." });
        }


        [HttpPost("UserDeletion")]
        [Authorize]
        public async Task<IActionResult> DeleteUser([FromQuery] int id)
        {
            int result = await _UserService.DeleteUser(id);

            if (result == 0) { return NotFound("User ID not found."); }
            else { return Ok(); }
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
