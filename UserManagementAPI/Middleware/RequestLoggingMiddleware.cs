using System.Diagnostics;

namespace UserManagementAPI.Middleware
{
    /// <summary>
    /// Middleware to log basic request/response information and timing.
    /// </summary>
    public class RequestLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<RequestLoggingMiddleware> _logger;

        public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var sw = Stopwatch.StartNew();
            var request = context.Request;

            _logger.LogInformation("Incoming request {method} {path}{query}", request.Method, request.Path, request.QueryString);

            // Copy the original response body stream to capture response size/status
            var originalBodyStream = context.Response.Body;

            try
            {
                await _next(context);
                sw.Stop();
                _logger.LogInformation("Handled {method} {path} responded {status} in {elapsed}ms",
                    request.Method,
                    request.Path,
                    context.Response.StatusCode,
                    sw.ElapsedMilliseconds);
            }
            catch (Exception ex)
            {
                sw.Stop();
                _logger.LogError(ex, "Request {method} {path} failed after {elapsed}ms", request.Method, request.Path, sw.ElapsedMilliseconds);
                throw;
            }
            finally
            {
                context.Response.Body = originalBodyStream;
            }
        }
    }

    public static class RequestLoggingExtensions
    {
        public static IApplicationBuilder UseRequestLogging(this IApplicationBuilder app)
        {
            return app.UseMiddleware<RequestLoggingMiddleware>();
        }
    }
}
