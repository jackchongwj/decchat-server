using Azure.Core;
using ChatroomB_Backend.Service;
using ChatroomB_Backend.Utils;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ChatroomB_Backend.Middleware
{
    public class TokenValidationMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IMemoryCache _cache;
        private readonly IConfiguration _config;

        public TokenValidationMiddleware(RequestDelegate next, IMemoryCache cache, IConfiguration config)
        {
            _next = next;
            _cache = cache;
            _config = config;
        }

        public async Task Invoke(HttpContext context)
        {
            // Access scoped services from the request's service provider
            ITokenService tokenService = context.RequestServices.GetService<ITokenService>()!;
            string accessToken;

            // Skip middleware for auth routes        
            if (context.Request.Path.StartsWithSegments("/api/Auth") || context.Request.Path.StartsWithSegments("/chatHub/negotiate"))
            {
                await _next(context);
                return;
            }

            // For SignalR requests (excluding negotiate), extract the access token from the query string
            if (context.Request.Path.StartsWithSegments("/chatHub"))
            {
                Console.WriteLine("HTTP Request with /chathub");
                accessToken = context.Request.Query["access_token"]!;
            }
            // For other requests, extract the JWT from the Authorization header
            else
            {
                accessToken = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last()!;
            }

            if (string.IsNullOrWhiteSpace(accessToken))
            {
                throw new UnauthorizedAccessException("Invalid access token");
            }

            try
            {
                // Validate the token and get ClaimsPrincipal
                ClaimsPrincipal principal = GetPrincipalFromToken(accessToken);

                // Extract userId and username from ClaimsPrincipal
                var userIdClaim = principal.Claims.FirstOrDefault(c => c.Type == "UserId")?.Value;
                var usernameClaim = principal.Claims.FirstOrDefault(c => c.Type == "Username")?.Value;

                if (userIdClaim == null || usernameClaim == null)
                {
                    throw new UnauthorizedAccessException("Invalid access token");
                }

                int userId = int.Parse(userIdClaim);

                await tokenService.ValidateAccessToken(userId, usernameClaim);

                // Cache the ClaimsPrincipal for performance
                string cacheKey = $"validation-{accessToken}";
                _cache.Set(cacheKey, principal, TimeSpan.FromMinutes(15));

                // Attach user information to HttpContext for downstream use
                context.Items["UserId"] = userId;
                context.Items["Username"] = usernameClaim;
                context.User = principal;
            }
            catch (SecurityTokenException ex)
            {
                throw new UnauthorizedAccessException("Invalid access token");
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to execute TokenValidationMiddleware.", ex);
            }

            await _next(context);
        }

        private ClaimsPrincipal GetPrincipalFromToken(string token)
        {
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = false,
                ValidateIssuerSigningKey = true,

                ValidIssuer = _config["JwtSettings:Issuer"],
                ValidAudience = _config["JwtSettings:Audience"],
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["JwtSettings:SecretKey"]!))
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out var securityToken);
            var jwtSecurityToken = securityToken as JwtSecurityToken;

            if (jwtSecurityToken == null || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
            {
                throw new UnauthorizedAccessException("Invalid access token");
            }

            return principal;
        }
    }
}
