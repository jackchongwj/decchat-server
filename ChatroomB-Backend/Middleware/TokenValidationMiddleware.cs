using System.IdentityModel.Tokens.Jwt;

namespace ChatroomB_Backend.Middleware
{
    public class TokenValidationMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IConfiguration _config;

        public TokenValidationMiddleware(RequestDelegate next, IConfiguration config)
        {
            _next = next;
            _config = config;
        }

        public async Task Invoke(HttpContext context)
        {
            // Skip middleware for auth routes
            if (context.Request.Path.StartsWithSegments("/api/Auth"))
            {
                await _next(context);
                return;
            }

            string token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();

            if (token != null)
        {
            bool userIsValid = await AttachUserToContext(context, token);
            if (!userIsValid)
            {
                context.Response.StatusCode = 401;
                await context.Response.WriteAsync("Invalid or inactive user.");
                return;
            }
        }

            await _next(context);
        }

        private async Task<bool> AttachUserToContext(HttpContext context, string token)
        {
            try
            {
                JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler();
                JwtSecurityToken jwtToken = handler.ReadToken(token) as JwtSecurityToken;

                if (jwtToken == null) return false ;

                string userIdClaim = jwtToken.Claims.FirstOrDefault(claim => claim.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")?.Value;
                string usernameClaim = jwtToken.Claims.FirstOrDefault(claim => claim.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name")?.Value;

                if (userIdClaim == null || usernameClaim == null) return false;

                int userId = int.Parse(userIdClaim);
                string username = usernameClaim;

                //return await _userService.ValidateUserAsync(userId, username);
                return true;
            }
            catch
            {
                return false;
            }
        }

    }
}
