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
        private readonly ITokenUtils _tokenUtil;
        private readonly ITokenService _tokenService;

        public AuthController
            (
            IAuthService authService,
            IAuthUtils authUtils,
            IUserService userService,
            ITokenUtils tokenUtil,
            ITokenService tokenService
            )
        {
            _authService = authService;
            _authUtils = authUtils;
            _userService = userService;
            _tokenUtil = tokenUtil;
            _tokenService = tokenService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            if (!ModelState.IsValid)
            {
                throw new ArgumentException("Invalid request data");
            }

            // Store the user object
            await _authService.AddUser(request.Username, request.Password, request.ProfileName!);

            return Ok(new { Message = "Registration successful!" });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            if(!ModelState.IsValid)
            {
                throw new ArgumentException("Invalid request data");
            }

            Users user = await _authService.Authenticate(request.Username, request.Password);

            if (user == null)
            {
                throw new UnauthorizedAccessException("Invalid username or password");
            }

            // Generate access token and refresh token
            string accessToken = _tokenUtil.GenerateAccessToken(user.UserId, user.UserName);
            RefreshToken refreshToken = _tokenUtil.GenerateRefreshToken(user.UserId);

            // Store refresh token in database
            await _tokenService.StoreRefreshToken(refreshToken);

            // Set the refresh token in a cookie
            CookieOptions cookieOptions = _tokenUtil.SetCookieOptions();
            Response.Cookies.Append("refreshToken", refreshToken.Token, cookieOptions);

            // Return access token and user Id in the response body
            return Ok(new
            {
                AccessToken = accessToken,
                UserId = user.UserId,
                Message = "Login successful!"
            });
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            // Retrieve refresh token from the request
            string refreshToken = Request.Cookies["refreshToken"];

            // Delete refresh token from database
            if(!string.IsNullOrEmpty(refreshToken)) 
            {
                await _tokenService.RemoveRefreshToken(refreshToken);
            }

            // Delete refresh token from client (cookie)
            CookieOptions cookieOptions = _tokenUtil.SetCookieOptions();
            Response.Cookies.Delete("refreshToken", cookieOptions);

            return Ok(new { Message = "Logout successful" });
        }
        
        [HttpPost("PasswordChange")]
        [Authorize]
        public async Task<IActionResult> ChangePassword([FromBody] PasswordChange model)
        {
            if (!ModelState.IsValid)
            {
                throw new ArgumentException("Invalid request data");
            }

            string username = _authUtils.ExtractUsernameFromJWT(HttpContext.User);

            await _authService.ChangePassword(username, model.CurrentPassword, model.NewPassword);

            return Ok(new { Message = "Password changed successfully." });
        }
    }
 
}