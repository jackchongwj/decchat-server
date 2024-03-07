using ChatroomB_Backend.Service;
using Microsoft.Data.SqlClient;
using System.ComponentModel.DataAnnotations;
using System.Net;

namespace ChatroomB_Backend.Middleware
{
    public class ExceptionHandlingMiddleware
    {

        private readonly RequestDelegate next;
        private readonly ILogger<ExceptionHandlingMiddleware> logger;

        public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
        {
            this.next = next;
            this.logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await next(context);
            }
            catch (SqlException ex)
            {
                await LogErrorMessage(context, "SQL Error Occurred: " + ex.Message);
            }
            catch (HttpRequestException ex)
            {
                await LogErrorMessage(context, "Http Request Failed: " + ex.Message);
            }
            catch (Exception ex)
            {
                await LogErrorMessage(context, ex.Message);
            }
            
        }

        private async Task LogErrorMessage(HttpContext context, string  errorMessage)
        {
            string controllerName = context.Request.RouteValues["controller"]?.ToString();
            IErrorHandleService errorHandleService = context.RequestServices.GetRequiredService<IErrorHandleService>();
            await errorHandleService.LogError(controllerName, errorMessage);
        }

    }
}
