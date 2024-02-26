using ChatroomB_Backend.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using ChatroomB_Backend.Models;
using System.IdentityModel.Tokens.Jwt;

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
        [AllowAnonymous]
        public async Task<IActionResult> RenewToken()
        {
            // Extract the expired access token from the Authorization header
            var authHeader = Request.Headers["Authorization"].FirstOrDefault();
            if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
            {
                return new UnauthorizedObjectResult(new { Error = "Authorization header is missing or invalid" });
            }

            // Extract the refresh token from the request cookie
            var refreshToken = Request.Cookies["refreshToken"];
            if (string.IsNullOrEmpty(refreshToken))
            {
                return new UnauthorizedObjectResult(new { Error = "Refresh token is missing" });
            }

            // Decode the token
            var expiredToken = authHeader.Substring("Bearer ".Length).Trim();
            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtToken = tokenHandler.ReadJwtToken(expiredToken);

            
            // Define the JWT claim type URIs
            var userIdClaimType = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier";
            var userNameClaimType = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name";

            // Retrieve the userId and username
            var userIdString = jwtToken.Claims.FirstOrDefault(claim => claim.Type == userIdClaimType)?.Value;
            var username = jwtToken.Claims.FirstOrDefault(claim => claim.Type == userNameClaimType)?.Value;

            // Attempt to parse and null checking
            if (!int.TryParse(userIdString, out var userId))
            {
                return new UnauthorizedObjectResult(new { Error = "Invalid user ID in token" });
            }

            if (string.IsNullOrEmpty(username))
            {
                return new UnauthorizedObjectResult(new { Error = "Invalid username in token" });
            }

            // Create refresh token object
            RefreshToken token = new RefreshToken
            {
                Token = refreshToken,
            };

            // Call token service to generate new access token after validating the refresh token
            try
            {
                var newAccessToken = await _tokenService.RenewAccessToken(token, userId, username);

                // Assuming GenerateAccessToken will throw an exception if it cannot generate a token
                return new OkObjectResult(new { AccessToken = newAccessToken });
            }
            catch (UnauthorizedAccessException ex)
            {
                return new UnauthorizedObjectResult(new { Error = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Error = "An error occurred while renewing the access token." });
            }
        }
    } 
}
