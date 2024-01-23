using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ChatroomB_Backend.Models;
using ChatroomB_Backend.Service;
using ChatroomB_Backend.Utils;

namespace ChatroomB_Backend.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IAuthUtils _authUtils;
        private readonly IUserService _userService;
        private readonly ITokenUtils _tokenUtils;
        private readonly ITokenService _tokenService;

        public AuthController
            (
            IAuthService authService,
            IAuthUtils authUtils,
            IUserService userService,
            ITokenUtils tokenUtils,
            ITokenService tokenService
            )
        {
            _authService = authService;
            _authUtils = authUtils;
            _userService = userService;
            _tokenUtils = tokenUtils;
            _tokenService = tokenService;
        }

        [HttpPost("IsUsernameUnique/{username}")]
        public async Task<IActionResult> IsUsernameUnique(string username)
        {
            var isUnique = await _userService.IsUsernameUnique(username);

            if (!isUnique)
            {
                return BadRequest(new { IsUnique = false, Message = "Username already exists." });  
            }

            return Ok();
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] string username, string password)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { Message = "Invalid request data" });
            }

            try
            {
                // Generate salt and hashed password
                string salt = _authUtils.GenerateSalt();

                string hashedPassword = _authUtils.HashPassword(password, salt);

                // Create a Users object
                Users user = new Users
                {
                    UserName = username,
                    HashedPassword = hashedPassword,
                    Salt = salt,
                };

                // Call the AddUser method in AuthService
                IActionResult result = await _authService.AddUser(user);

                return result;
            }
            catch
            {
                return StatusCode(500, new { Message = "Internal Server Error" });
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] string username, string password)
        {
            if(!ModelState.IsValid)
            {
                return BadRequest(new { Message = "Invalid request data" });
            }

            try
            {
                string salt = await _authService.GetSalt(username);

                string hashedPassword = _authUtils.HashPassword(password, salt);

                bool isAuthenticated = await _authService.VerifyPassword(username, hashedPassword);

                if (isAuthenticated)
                {
                    int userId = await _userService.GetUserId(username);

                    var accessToken = _tokenUtils.GenerateAccessToken(username);

                    var refreshToken = _tokenUtils.GenerateRefreshToken();

                    await _tokenService.StoreRefreshToken(new RefreshToken
                    {
                        UserId = userId,
                        Token = refreshToken,
                        ExpiredDateTime = DateTime.UtcNow.AddDays(7)
                    });

                    return Ok(new { Message = "Login successful!", UserId = userId, AccessToken = accessToken, RefreshToken = refreshToken });
                }

                return Unauthorized(new { Message = "Invalid username or password" });
            }
            
            catch
            {
                return StatusCode(500, new { Message = "Internal Server Error" });
            }
        }
    }
}