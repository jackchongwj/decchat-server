using ChatroomB_Backend.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using ChatroomB_Backend.Models;
using System.IdentityModel.Tokens.Jwt;
using ChatroomB_Backend.Utils;
using Microsoft.IdentityModel.Tokens;

namespace ChatroomB_Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TokenController : ControllerBase
    {
        private readonly ITokenService _tokenService;
        private readonly ITokenUtils _tokenUtil;

        public TokenController(ITokenService tokenService, ITokenUtils tokenUtil)
        {
            _tokenService = tokenService;
            _tokenUtil = tokenUtil;
        }

        [HttpPost("RenewToken")]
        [AllowAnonymous]
        public async Task<IActionResult> RenewToken()
        {
            // Get Cookie Refresh Token
            string refreshToken = HttpContext.Request.Cookies["refreshToken"]!;

            if (string.IsNullOrWhiteSpace(refreshToken))
            {
                return Unauthorized("Refresh token is missing");
            }

            // Retrieve userId and username from HttpContext, attached by the middleware
            if (!HttpContext.Items.TryGetValue("UserId", out var userIdObj) ||
                !HttpContext.Items.TryGetValue("Username", out var usernameObj))
            {
                return Unauthorized("User information is missing in the request context");
            }

            int userId = (int)userIdObj!;
            string username = (string)usernameObj!;

            // Validate and update refresh token expiry
            await _tokenService.UpdateRefreshToken(refreshToken);

            // Generate a new access token
            string newAccessToken = _tokenUtil.GenerateAccessToken(userId, username);

            return Ok(new { AccessToken = newAccessToken });
        }
    }
}
