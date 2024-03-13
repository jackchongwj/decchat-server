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
                throw new ArgumentException();
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
                throw new ArgumentException();
            }

            Users user = await _authService.Authenticate(request.Username, request.Password);

            // Generate access token and refresh token
            string accessToken = _tokenUtil.GenerateAccessToken(user.UserId, user.UserName);
            RefreshToken refreshToken = _tokenUtil.GenerateRefreshToken(user.UserId);

            // Store refresh token in database
            await _tokenService.StoreRefreshToken(refreshToken);

            // Return both tokens in the response body
            return Ok(new
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken.Token,
                Message = "Login successful!"
            });
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            // Retrieve refresh token from the request
            string refreshToken = Request.Headers["X-Refresh-Token"].FirstOrDefault()!;

            if (string.IsNullOrWhiteSpace(refreshToken))
            {
                throw new ArgumentException("Refresh token is required.");
            }

            // Delete refresh token from database
            await _tokenService.RemoveRefreshToken(refreshToken);

            return Ok(new { Message = "Logout successful" });
        }
        
        [HttpPost("PasswordChange")]
        [Authorize]
        public async Task<IActionResult> ChangePassword([FromBody] PasswordChange model)
        {
            if (!ModelState.IsValid)
            {
                throw new ArgumentException();
            }

            string username = _authUtils.ExtractUsernameFromJWT(HttpContext.User);

            await _authService.ChangePassword(username, model.CurrentPassword, model.NewPassword);

            return Ok(new { Message = "Password changed successfully." });
        }


    }
 
}