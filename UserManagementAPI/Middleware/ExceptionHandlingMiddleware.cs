using System.Net;
using System.Text.Json;
using UserManagementAPI.Exceptions;
using UserManagementAPI.Models;

namespace UserManagementAPI.Middleware
{
    /// <summary>
    /// Global exception handling middleware
    /// </summary>
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;

        public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled exception caught by middleware");
                await HandleExceptionAsync(context, ex);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";

            var response = new ErrorResponse();

            switch (exception)
            {
                case UserNotFoundException ex:
                    context.Response.StatusCode = StatusCodes.Status404NotFound;
                    response.StatusCode = StatusCodes.Status404NotFound;
                    response.Message = "User Not Found";
                    response.Details = ex.Message;
                    break;

                case DuplicateEmailException ex:
                    context.Response.StatusCode = StatusCodes.Status409Conflict;
                    response.StatusCode = StatusCodes.Status409Conflict;
                    response.Message = "Duplicate Email";
                    response.Details = ex.Message;
                    break;

                case InvalidUserDataException ex:
                    context.Response.StatusCode = StatusCodes.Status400BadRequest;
                    response.StatusCode = StatusCodes.Status400BadRequest;
                    response.Message = "Invalid User Data";
                    response.Details = ex.Message;
                    break;

                case ValidationException ex:
                    context.Response.StatusCode = StatusCodes.Status400BadRequest;
                    response.StatusCode = StatusCodes.Status400BadRequest;
                    response.Message = "Validation Error";
                    response.Details = ex.Message;
                    response.Errors = ex.Errors;
                    break;

                case ArgumentNullException ex:
                    context.Response.StatusCode = StatusCodes.Status400BadRequest;
                    response.StatusCode = StatusCodes.Status400BadRequest;
                    response.Message = "Null Argument Provided";
                    response.Details = ex.ParamName != null 
                        ? $"Parameter '{ex.ParamName}' cannot be null." 
                        : ex.Message;
                    break;

                case ArgumentException ex:
                    context.Response.StatusCode = StatusCodes.Status400BadRequest;
                    response.StatusCode = StatusCodes.Status400BadRequest;
                    response.Message = "Invalid Argument";
                    response.Details = ex.Message;
                    break;

                default:
                    context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                    response.StatusCode = StatusCodes.Status500InternalServerError;
                    response.Message = "Internal Server Error";
                    response.Details = "An unexpected error occurred. Please contact support.";
                    break;
            }

            var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
            var json = JsonSerializer.Serialize(response, options);

            return context.Response.WriteAsJsonAsync(response);
        }
    }

    /// <summary>
    /// Extension method to configure exception handling middleware
    /// </summary>
    public static class ExceptionHandlingExtensions
    {
        public static IApplicationBuilder UseExceptionHandlingMiddleware(this IApplicationBuilder app)
        {
            return app.UseMiddleware<ExceptionHandlingMiddleware>();
        }
    }
}
