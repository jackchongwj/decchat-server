using ChatroomB_Backend.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

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

        //[HttpGet("RenewToken")]
        //public async Task<IActionResult> RenewToken()
        //{
        //    // Extract the refresh token from the request, e.g., from a cookie
        //    var refreshToken = Request.Cookies["refreshToken"];

        //    if (string.IsNullOrEmpty(refreshToken))
        //    {
        //        return new UnauthorizedObjectResult(new { Error = "Refresh token is missing" });
        //    }

        //    // Validate the refresh token and generate a new access token
        //    var newAccessToken = await _tokenService.RenewAccessToken(refreshToken);

        //    if (newAccessToken == null)
        //    {
        //        return new UnauthorizedObjectResult(new { Error = "Invalid or expired refresh token" });
        //    }

        //    return Ok(new { AccessToken = newAccessToken });
        //}
    }
}
