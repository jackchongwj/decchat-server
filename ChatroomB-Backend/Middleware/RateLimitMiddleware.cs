using Microsoft.AspNetCore.Http;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;

namespace ChatroomB_Backend.Middleware
{
    public class RateLimitMiddleware
    {
        private readonly RequestDelegate _next;

        public RateLimitMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            await _next(context);

            if (context.Response.StatusCode == (int)HttpStatusCode.TooManyRequests)
            {
                // Ensure the response body is cleared
                context.Response.Clear();

                // Customize the response
                context.Response.ContentType = "application/json";
                var retryAfter = context.Response.Headers["Retry-After"].FirstOrDefault();
                var content = JsonSerializer.Serialize(new
                {
                    Message = "Rate limit exceeded. Please try again later.",
                    RetryAfter = retryAfter
                });

                await context.Response.WriteAsync(content);
            }
        }
    }

}
