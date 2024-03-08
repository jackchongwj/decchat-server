using Microsoft.AspNetCore.Mvc;
using ChatroomB_Backend.Models;
using ChatroomB_Backend.Service;
using ChatroomB_Backend.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;

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
        public async Task<IActionResult> SearchByProfileName(string profileName)
        {
            // get all data from JWT token
            ClaimsIdentity? identity = HttpContext.User.Identity as ClaimsIdentity;
            if (identity == null)
            {
                return Unauthorized("User identity not found");
            }

            Claim userIdClaim = identity.FindFirst("userId")!; // Claim name should match the one used in the token creation
            if (userIdClaim == null)
            {
                return Unauthorized("User ID claim not found in the token");
            }

            if (!int.TryParse(userIdClaim.Value, out int userId))
            {
                return BadRequest("Invalid User ID claim value");
            }


            if (!profileName.IsNullOrEmpty())
            {
                IEnumerable<UserSearchDetails> GetUserByName = await _UserService.GetByName(profileName.Trim(), userId);

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

            return Ok(GetFriendRequest);

        }

        [HttpGet("UserDetails")]
        [Authorize]
        public async Task<ActionResult<Users>> GetUserById()
        {
            int id = (int)HttpContext.Items["UserId"]!;

            Users user = await _UserService.GetUserById(id);

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
