using ChatroomB_Backend.Service;
using ChatroomB_Backend.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using System;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;

namespace ChatroomB_Backend.Middleware
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;

        public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            int userId = 0; // Default value indicating unauthenticated or invalid request
            string controllerName = context.Request.RouteValues["controller"]?.ToString() ?? "UnknownController";
            string message;
            HttpStatusCode statusCode;

            // Try to resolve IAuthUtils and extract userId only if the user is authenticated
            if (context.User.Identity.IsAuthenticated)
            {
                var authUtils = context.RequestServices.GetRequiredService<IAuthUtils>();
                try
                {
                    userId = authUtils.ExtractUserIdFromJWT(context.User);
                }
                catch (Exception authException)
                {
                    _logger.LogWarning(authException, "Failed to extract userId from JWT");
                    // In this case, keep the default userId value
                }
            }

            switch (exception)
            {
                case SqlException _:
                    statusCode = HttpStatusCode.InternalServerError;
                    message = "A database error occurred.";
                    break;
                case HttpRequestException _:
                    statusCode = HttpStatusCode.BadGateway;
                    message = "External service request failed.";
                    break;
                case ArgumentException _:
                    statusCode = HttpStatusCode.BadRequest;
                    message = exception.Message ?? "Invalid request data";
                    break;
                case UnauthorizedAccessException _:
                    statusCode = HttpStatusCode.Unauthorized;
                    message = exception.Message;
                    break;
                case KeyNotFoundException _:
                    statusCode = HttpStatusCode.NotFound;
                    message = exception.Message;
                    break;
                case InvalidOperationException _:
                    statusCode = HttpStatusCode.Conflict;
                    message = exception.Message;
                    break;
                default:
                    statusCode = HttpStatusCode.InternalServerError;
                    message = "An unexpected error occurred.";
                    break;
            }

            _logger.LogError(exception, exception.Message);

            // Log the error to MongoDB using the resolved userId (or the default value)
            IErrorHandleService errorHandleService = context.RequestServices.GetRequiredService<IErrorHandleService>();
            await errorHandleService.LogError(controllerName, userId, exception.Message);

            // Return a Json object as response
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)statusCode;
            await context.Response.WriteAsync(JsonSerializer.Serialize(message));
        }
    }
}