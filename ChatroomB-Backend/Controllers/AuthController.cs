using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ChatroomB_Backend.Models;
using ChatroomB_Backend.Service;
using ChatroomB_Backend.Utils;
using ChatroomB_Backend.DTO;
using Microsoft.AspNetCore.Cors;

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
        public async Task<ActionResult> Register([FromBody] AuthRequest request)
        {
            // Check if request data valid
            if (!ModelState.IsValid)
            {
                return new BadRequestObjectResult(new { Error = "Invalid request data" });
            }

            // Check if username exists
            bool isUnique = await _userService.DoesUsernameExist(request.Username);

            if (!isUnique)
            {
                return new ConflictObjectResult(new { Error = "Duplicate username detected" });
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
                return new BadRequestObjectResult(new { Error = "Invalid request data" });
            }

            try
            {
                // Check if username exists
                bool doesNotExist = await _userService.DoesUsernameExist(request.Username);

                if (doesNotExist)
                {
                    return new UnauthorizedObjectResult(new { Error = "Invalid username or password" });
                }

                // Get salt from user object in database
                string salt = await _authService.GetSalt(request.Username);

                // Hash the input password with salt
                string hashedPassword = _authUtils.HashPassword(request.Password, salt);

                // Authenticate user
                bool isAuthenticated = await _authService.VerifyPassword(request.Username, hashedPassword);

                if (!isAuthenticated)
                {
                    return new UnauthorizedObjectResult(new { Error = "Invalid username or password" });
                }

                // Get user Id
                int userId = await _userService.GetUserId(request.Username);

                // Generate access token
                string accessToken = _tokenUtils.GenerateAccessToken(request.Username);

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
                return new OkObjectResult(new
                {
                    AccessToken = accessToken,
                    UserId = userId,
                    Message = "Login successful!"
                });
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

                return new OkObjectResult (new { Message = "Logout successful" });
            }
            catch
            {
                return StatusCode(500, new { Error = "Internal Server Error" });
            }
        }
    }
}