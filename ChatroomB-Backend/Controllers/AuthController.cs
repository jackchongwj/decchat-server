using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ChatroomB_Backend.Models;
using ChatroomB_Backend.Service;
using ChatroomB_Backend.Utils;
using ChatroomB_Backend.DTO;

namespace ChatroomB_Backend.Controllers
{
    [Route("api/[controller]")]
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

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            if (!ModelState.IsValid)
            {
                throw new ArgumentException("Invalid request data");
            }

            // Check if username exists
            bool isUnique = await _userService.DoesUsernameExist(request.Username);

            if (isUnique)
            {
                throw new ArgumentException("Duplicate username detected");
            }

            // Generate salt and hashed password
            string salt = _authUtils.GenerateSalt();
            string hashedPassword = _authUtils.HashPassword(request.Password, salt);

            // Create a user object
            Users user = new Users
            {
                UserName = request.Username,
                ProfileName = request.ProfileName,
                HashedPassword = hashedPassword,
                Salt = salt,
            };

            // Store the user object
            IActionResult result = await _authService.AddUser(user);

            return result;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            if(!ModelState.IsValid)
            {
                throw new ArgumentException("Invalid request data");
            }

            // Check if username exists
            bool doesExist = await _userService.DoesUsernameExist(request.Username);

            if (!doesExist)
            {
                throw new UnauthorizedAccessException("Invalid username or password");
            }

            // Get salt from user object in database
            string salt = await _authService.GetSalt(request.Username);

            // Hash the input password with salt
            string hashedPassword = _authUtils.HashPassword(request.Password, salt);

            // Authenticate user
            bool isAuthenticated = await _authService.VerifyPassword(request.Username, hashedPassword);

            if (!isAuthenticated)
            {
                throw new UnauthorizedAccessException("Invalid username or password");
            }

            // Get user Id
            int userId = await _userService.GetUserId(request.Username);

            // Generate access token
            string accessToken = _tokenUtils.GenerateAccessToken(userId, request.Username);

            // Generate refresh token
            string refreshToken = _tokenUtils.GenerateRefreshToken();

            // Set the refresh token in a cookie
            CookieOptions cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Expires = DateTime.UtcNow.AddDays(7),
                Secure = true,
                SameSite = SameSiteMode.None
            };

            HttpContext.Response.Cookies.Append("refreshToken", refreshToken, cookieOptions);

            // Store the refresh token in database
            RefreshToken token = new RefreshToken
            {
                UserId = userId,
                Token = refreshToken,
                ExpiredDateTime = DateTime.UtcNow.AddDays(7)
            };

            await _tokenService.StoreRefreshToken(token);

            // Return access token and user Id in the response body
            return Ok(new
            {
                AccessToken = accessToken,
                UserId = userId,
                Message = "Login successful!"
            });
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            // Retrieve the refresh token from the request
            string refreshToken = Request.Cookies["refreshToken"];

            if(!string.IsNullOrEmpty(refreshToken)) 
            {
                // Create a refresh token object
                RefreshToken token = new RefreshToken
                {
                    Token = refreshToken
                };

                // Delete refresh token from database
                await _tokenService.RemoveRefreshToken(token);
            }

            // Replicate the cookie options
            CookieOptions cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None
            };

            // Delete refresh token from client (cookie)
            HttpContext.Response.Cookies.Delete("refreshToken", cookieOptions);

            return Ok(new { Message = "Logout successful" });
        }
        
        [HttpPost("PasswordChange")]
        [Authorize]
        public async Task<IActionResult> ChangePassword(int id, [FromBody] PasswordChange model)
        {
            if (!ModelState.IsValid)
            {
                throw new ArgumentException("Invalid request data");
            }

            bool result = await _authService.ChangePassword(id, model.CurrentPassword, model.NewPassword);

            if (!result)
            {
                // This means the current password did not match
                throw new ArgumentException("Current password is incorrect or user not found.");
            }   

            // If the password was successfully changed
            return Ok(new { Message = "Password changed successfully." });

        }
    }
}