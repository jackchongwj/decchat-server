using System.ComponentModel.DataAnnotations;
using System.Net;

namespace ChatroomB_Backend.Middleware
{
    public class ExecptionHandlingMiddleware
    {

        private readonly RequestDelegate next;
        private readonly ILogger<ExecptionHandlingMiddleware> logger;

        public ExecptionHandlingMiddleware(RequestDelegate next, ILogger<ExecptionHandlingMiddleware> logger)
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
            catch (Exception ex)
            {
                //logger.LogError(ex, "Exception occurred: {ex.Message}");

                //context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                //context.Response.ContentType = "text/plain";
                //await context.Response.WriteAsync("An internal server error occurred.");

                HttpStatusCode code;
                switch (ex) 
                {
                    case UnauthorizedAccessException:
                        code = HttpStatusCode.Unauthorized;
                        break;

                    case ValidationException:
                        code = HttpStatusCode.Forbidden;
                        break;

                    //default: 
                    //    code = HttpStatusCode.InternalServerError; 
                    //    await context.Response.WriteAsync("An unexpected error occurred.");
                    //    break;
                }
            }


            //404 no found
            if (context.Response.StatusCode == (int)HttpStatusCode.NotFound)
            {
                context.Response.ContentType = "text/plain";
                await context.Response.WriteAsync("Not found.");
            }
        }
    }
}
