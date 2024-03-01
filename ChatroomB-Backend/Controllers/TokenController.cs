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
            string authHeader = Request.Headers["Authorization"].FirstOrDefault();

            if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
            {
                return Unauthorized("Authorization header is missing or invalid");
            }

            // Extract the refresh token from the request cookie
            string refreshToken = Request.Cookies["refreshToken"];

            if (string.IsNullOrEmpty(refreshToken))
            {
                return Unauthorized("Refresh token is missing");
            }

            // Decode the token
            string expiredToken = authHeader.Substring("Bearer ".Length).Trim();
            JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
            JwtSecurityToken jwtToken = tokenHandler.ReadJwtToken(expiredToken);

            // Define the JWT claim type URIs
            string userIdClaimType = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier";
            string userNameClaimType = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name";

            // Retrieve the userId and username
            string userIdString = jwtToken.Claims.FirstOrDefault(claim => claim.Type == userIdClaimType)?.Value;
            string username = jwtToken.Claims.FirstOrDefault(claim => claim.Type == userNameClaimType)?.Value;

            // Attempt to parse and null checking
            if (!int.TryParse(userIdString, out int userId))
            {
                return Unauthorized("Invalid user ID in token");
            }

            if (string.IsNullOrEmpty(username))
            {
                return Unauthorized("Invalid username in token");
            }

            // Create refresh token object
            RefreshToken token = new RefreshToken
            {
                Token = refreshToken,
            };

            // Call token service to generate new access token after validating the refresh token
            try
            {
                string newAccessToken = await _tokenService.RenewAccessToken(token, userId, username);

                // Assuming GenerateAccessToken will throw an exception if it cannot generate a token
                return Ok(new { AccessToken = newAccessToken });
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
