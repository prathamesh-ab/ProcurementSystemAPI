using System.Net;
using System.Text.Json;
using static ProcurementSystem.API.Exceptions.CustomExceptions;

namespace ProcurementSystem.API.Middleware
{
    public class GlobalExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionMiddleware> _logger;
        private readonly IWebHostEnvironment _env;

        public GlobalExceptionMiddleware(
            RequestDelegate next,
            ILogger<GlobalExceptionMiddleware> logger,
            IWebHostEnvironment env)
        {
            _next = next;
            _logger = logger;
            _env = env;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled exception. Path: {Path}", context.Request.Path);
                await HandleExceptionAsync(context, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";

            var response = new
            {
                Success = false,
                Message = "",
                Errors = new List<string>(),
                TraceId = context.TraceIdentifier
            };

            switch (exception)
            {
                case NotFoundException notFoundEx:
                    context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                    response = response with { Message = notFoundEx.Message };
                    break;

                case DuplicateException duplicateEx:
                    context.Response.StatusCode = (int)HttpStatusCode.Conflict;
                    response = response with { Message = duplicateEx.Message };
                    break;

                case DatabaseException dbEx:
                    context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    response = response with
                    {
                        Message = "A database error occurred",
                        Errors = _env.IsDevelopment()
                            ? new List<string> { dbEx.InnerException?.Message ?? dbEx.Message }
                            : new List<string>()
                    };
                    break;

                default:
                    context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    response = response with
                    {
                        Message = "An internal server error occurred",
                        Errors = _env.IsDevelopment()
                            ? new List<string> { exception.Message }
                            : new List<string>()
                    };
                    break;
            }

            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };

            var json = JsonSerializer.Serialize(response, options);
            await context.Response.WriteAsync(json);
        }
    }
}
