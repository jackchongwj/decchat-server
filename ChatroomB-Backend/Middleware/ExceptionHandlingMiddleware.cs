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
        private readonly IAuthUtils _authUtils;

        public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger, IAuthUtils authUtils)
        {
            _next = next;
            _logger = logger;
            _authUtils = authUtils;
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
            HttpStatusCode statusCode;
            string message;

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
                case UnauthorizedAccessException _:
                    statusCode = HttpStatusCode.Unauthorized;
                    message = "Access denied.";
                    break;
                default:
                    statusCode = HttpStatusCode.InternalServerError;
                    message = "An unexpected error occurred.";
                    break;
            }

            _logger.LogError(exception, exception.Message);

            // Log the error to MongoDB
            int userId = _authUtils.ExtractUserIdFromJWT(context.User);
            string controllerName = context.Request.RouteValues["controller"]?.ToString() ?? "UnknownController";
            IErrorHandleService errorHandleService = context.RequestServices.GetRequiredService<IErrorHandleService>();
            await errorHandleService.LogError(controllerName, userId, exception.Message);

            var response = new { error = message };
            var jsonResponse = JsonSerializer.Serialize(response);

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)statusCode;

            await context.Response.WriteAsync(jsonResponse);
        }
    }
}
