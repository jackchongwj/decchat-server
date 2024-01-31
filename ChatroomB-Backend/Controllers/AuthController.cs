using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ChatroomB_Backend.Models;
using ChatroomB_Backend.Service;
using ChatroomB_Backend.Utils;
using ChatroomB_Backend.DTO;

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

        [HttpGet("IsUsernameUnique")]
        public async Task<ActionResult> IsUsernameUnique(string username)
        {
            var isUnique = await _userService.IsUsernameUnique(username);

            if (!isUnique)
            {
                return new BadRequestObjectResult(new { IsUnique = false, Message = "Username already exists." });  
            }

            return Ok();
        }

        [HttpPost("register")]
        public async Task<ActionResult> Register([FromBody] AuthRequest request)
        {
            // Check if request data valid
            if (!ModelState.IsValid)
            {
                return new BadRequestObjectResult(new { Message = "Invalid request data" });
            }

            // Check if username exists
            var isUnique = await _userService.IsUsernameUnique(request.Username);

            if (!isUnique)
            {
                return new BadRequestObjectResult(new { Message = "Username already exists." });
            }

            // Generate salt, hash password, and store user object
            try
            {
                // Generate salt and hashed password
                string salt = _authUtils.GenerateSalt();
                string hashedPassword = _authUtils.HashPassword(request.Password, salt);

                // Create a user object
                Users user = new Users
                {
                    UserName = request.Username,
                    HashedPassword = hashedPassword,
                    Salt = salt,
                };

                // Store the user object
                ActionResult result = await _authService.AddUser(user);

                return result;
            }
            catch
            {
                return StatusCode(500, new { Error = "Internal Server Error" });
            }
        }

        [HttpPost("login")]
        public async Task<ActionResult> Login([FromBody] AuthRequest request)
        {
            if(!ModelState.IsValid)
            {
                return new BadRequestObjectResult(new { Message = "Invalid request data" });
            }

            try
            {
                // Check if username exists
                bool doesNotExist = await _userService.IsUsernameUnique(request.Username);

                if (doesNotExist)
                {
                    return new UnauthorizedObjectResult(new { Message = "Invalid username or password" });
                }

                // Get salt from user object in database
                string salt = await _authService.GetSalt(request.Username);

                // Hash the input password with salt
                string hashedPassword = _authUtils.HashPassword(request.Password, salt);

                // Authenticate user
                bool isAuthenticated = await _authService.VerifyPassword(request.Username, hashedPassword);

                if (!isAuthenticated)
                {
                    return new UnauthorizedObjectResult(new { Message = "Invalid username or password" });
                    
                }

                // Get user Id
                int userId = await _userService.GetUserId(request.Username);

                // Generate access token
                string accessToken = _tokenUtils.GenerateAccessToken(request.Username);

                // Generate refresh token
                string refreshToken = _tokenUtils.GenerateRefreshToken();

                // Store refresh token in database
                await _tokenService.StoreRefreshToken(new RefreshToken
                {
                    UserId = userId,
                    Token = refreshToken,
                    ExpiredDateTime = DateTime.UtcNow.AddDays(7)
                });

                // Set the access token in the Authorization header
                HttpContext.Response.Headers.Append("Authorization", $"Bearer {accessToken}");

                // Set the refresh token in a cookie
                HttpContext.Response.Cookies.Append("refreshToken", refreshToken, new CookieOptions
                {
                    HttpOnly = true,
                    Expires = DateTime.UtcNow.AddDays(7),
                    Path = "/"
                });

                // Set the user Id in a cookie
                HttpContext.Response.Cookies.Append("userId", userId.ToString(), new CookieOptions
                {
                    HttpOnly = true,
                    Expires = DateTime.UtcNow.AddDays(7),
                    Path = "/"
                });

                // Return user Id, access token, and refresh token
                return new OkObjectResult(new { Message = "Login successful!"});
            }
            
            catch
            {
                return StatusCode(500, new { Error = "Internal Server Error" });
            }
        }

        [HttpPost("logout")]
        [Authorize]
        public async Task<ActionResult> Logout()
        {
            try
            {
                // Delete access token from client (header)
                Response.Headers.Remove("Authorization");

                // Retrieve the refresh token from the request
                string refreshToken = Request.Cookies["refresh_token"];

                // Delete refresh token from client (cookie)
                Response.Cookies.Delete("refresh_token");

                // Create a refresh token object
                var token = new RefreshToken
                {
                    Token = refreshToken
                };

                // Delete refresh token from database
                await _tokenService.RemoveRefreshToken(token);

                return Ok(new { Message = "Logout successful" });
            }
            catch
            {
                return StatusCode(500, new { Error = "Internal Server Error" });
            }
        }
        
        [HttpPost("PasswordChange")]
        public async Task<IActionResult> ChangePassword(int id, [FromBody] PasswordChange model)
        {
            // Check if the model is valid
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var result = await _authService.ChangePassword(id, model.CurrentPassword, model.NewPassword);
                if (!result)
                {
                    // This means the current password did not match
                    return BadRequest("Current password is incorrect or user not found.");
                }

                // If the password was successfully changed
                return Ok("Password changed successfully.");
            }
            catch (Exception ex)
            {
                // Log the exception details for debugging purposes
                // Consider using a logging framework or service
                Console.WriteLine(ex.Message);

                // Return a generic error message to avoid exposing sensitive details
                return StatusCode(500, "An error occurred while changing the password.");
            }
        }
    }
}