using Azure.Core;
using ChatroomB_Backend.Service;
using ChatroomB_Backend.Utils;
using Microsoft.Extensions.Caching.Memory;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace ChatroomB_Backend.Middleware
{
    public class TokenValidationMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IConfiguration _config;
        private readonly IMemoryCache _cache;
        private readonly ITokenUtils _tokenUtil;
        private readonly ITokenService _tokenService;

        public TokenValidationMiddleware(RequestDelegate next, IConfiguration config, IMemoryCache cache, ITokenUtils tokenUtil, ITokenService tokenService)
        {
            _next = next;
            _config = config;
            _cache = cache;
            _tokenUtil = tokenUtil;
            _tokenService = tokenService;
        }

        public async Task Invoke(HttpContext context)
        {
            string accessToken;
            // Skip middleware for auth routes        
            if (context.Request.Path.StartsWithSegments("/api/Auth") || context.Request.Path.StartsWithSegments("/chatHub/negotiate"))
            {
                //string connectionId = Context.ConnectionId;
                await _next(context);
                return;
            }

            if (context.Request.Path.StartsWithSegments("/chatHub"))
            {
                accessToken = context.Request.Query["access_token"];
            }
            else
            {
                // Get JWT Access Token
                accessToken = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last()!;
            }
            
            if (string.IsNullOrWhiteSpace(accessToken))
            {
                context.Response.StatusCode = 401; // Unauthorized
                await context.Response.WriteAsync("Access token is missing");
                return;
            }

            // Attempt to retrieve cached validation results
            string cacheKey = $"validation-{accessToken}";
            if (_cache.TryGetValue(cacheKey, out (int userId, string username) cachedResult))
            {
                List<Claim> claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, cachedResult.userId.ToString()),
                };
                ClaimsIdentity identity = new ClaimsIdentity(claims, "Bearer");
                ClaimsPrincipal principal = new ClaimsPrincipal(identity);

                context.User = principal; // This is the key step

                context.Items["UserId"] = cachedResult.userId;
                context.Items["Username"] = cachedResult.username;
            }
            else
            {
                // Decode Access Token
                var decodedToken = DecodeAccessToken(accessToken);
                if (decodedToken == null)
                {
                    context.Response.StatusCode = 401; // Unauthorized
                    await context.Response.WriteAsync("Invalid access token");
                    return;
                }

                // Validate Access Token (Check if username in JWT == username in DB WHERE userId in DB == userID in JWT)
                await _tokenService.ValidateAccessToken(decodedToken.Value.userId, decodedToken.Value.username);

                // Attach User to Context
                context.Items["UserId"] = decodedToken.Value.userId;
                context.Items["Username"] = decodedToken.Value.username;

                // Create and assign a ClaimsPrincipal from the validated token
                List<Claim> claims = new List<Claim>
                {
                    new Claim("UserId", decodedToken.Value.userId.ToString()),
                };
                ClaimsIdentity identity = new ClaimsIdentity(claims, "Bearer");
                ClaimsPrincipal principal = new ClaimsPrincipal(identity);

                context.User = principal; // This is the key step
            }

            await _next(context);
        }

        private (int userId, string username)? DecodeAccessToken(string accessToken)
        {
            JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler();
            JwtSecurityToken? jwtToken = handler.ReadToken(accessToken) as JwtSecurityToken;

            if (jwtToken == null) return null;

            // Retrieve username and userId
            string? userIdClaim = jwtToken.Claims.FirstOrDefault(claim => claim.Type == "UserId")?.Value;
            string? usernameClaim = jwtToken.Claims.FirstOrDefault(claim => claim.Type == "Username")?.Value;

            // Check for null or invalid values before returning
            if (userIdClaim == null || usernameClaim == null) return null;
            if (!int.TryParse(userIdClaim, out int userId) || userId == 0) return null;

            return (userId, usernameClaim);
        }
    }
}
