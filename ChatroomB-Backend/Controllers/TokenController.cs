using ChatroomB_Backend.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using ChatroomB_Backend.Models;

namespace ChatroomB_Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TokenController : ControllerBase
    {
        private readonly ITokenService _tokenService;

        public TokenController(ITokenService tokenService)
        {
            _tokenService = tokenService;
        }

        [HttpPost("RenewToken")]
        [Authorize]
        public async Task<IActionResult> RenewToken()
        {
            // Extract the refresh token from the request cookie
            var refreshToken = Request.Cookies["refreshToken"];
            
            if (string.IsNullOrEmpty(refreshToken))
            {
                return new UnauthorizedObjectResult(new { Error = "Refresh token is missing" });
            }

            // Extract the refresh token from httpcontext (middleware)
            var userId = HttpContext.Items["UserId"] as int?;

            if (!userId.HasValue)
            {
                return new UnauthorizedObjectResult(new { Error = "User ID is missing" });
            }

            // Create refresh token object
            RefreshToken token = new RefreshToken
            {
                Token = refreshToken,
            };

            // Validate the refresh token and generate a new access token
            var newAccessToken = await _tokenService.RenewAccessToken(token, userId.Value);

            if (newAccessToken == null)
            {
                return new UnauthorizedObjectResult(new { Error = "Invalid or expired refresh token" });
            }

            return new OkObjectResult(new { AccessToken = newAccessToken });
        }
    } 
}
