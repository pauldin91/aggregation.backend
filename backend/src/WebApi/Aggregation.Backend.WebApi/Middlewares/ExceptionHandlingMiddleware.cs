namespace Aggregation.Backend.WebApi.Middlewares
{
    public class ExceptionHandlingMiddleware(RequestDelegate next)
    {
        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await next(context);
            }
            catch (Exception)
            {
                await HandleExceptionAsync(context);

            }
        }

        private async Task HandleExceptionAsync(HttpContext context)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;

            var errorResponse = new
            {
                context.Response.StatusCode,
                Message = "An unexpected error occurred.",
            };

            await context.Response.WriteAsJsonAsync(errorResponse, CancellationToken.None);
        }



    }
}
