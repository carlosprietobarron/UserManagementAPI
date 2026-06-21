namespace UserManagementAPI.Middleware
{
    /// <summary>
    /// Simple API key authentication middleware. Expects header `X-Api-Key` with configured key.
    /// If no key configured, middleware allows requests (safe default for local development).
    /// </summary>
    public class ApiKeyAuthenticationMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ApiKeyAuthenticationMiddleware> _logger;
        private readonly string? _apiKey;

        public ApiKeyAuthenticationMiddleware(RequestDelegate next, ILogger<ApiKeyAuthenticationMiddleware> logger, IConfiguration config)
        {
            _next = next;
            _logger = logger;
            _apiKey = config["ApiKey"]; // optional
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // If no api key configured, skip authentication
            if (string.IsNullOrEmpty(_apiKey))
            {
                await _next(context);
                return;
            }

            if (!context.Request.Headers.TryGetValue("X-Api-Key", out var extractedApiKey))
            {
                _logger.LogWarning("Missing X-Api-Key header");
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await context.Response.WriteAsJsonAsync(new { message = "API key required" });
                return;
            }

            if (!string.Equals(extractedApiKey, _apiKey, StringComparison.Ordinal))
            {
                _logger.LogWarning("Invalid API key provided");
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await context.Response.WriteAsJsonAsync(new { message = "Invalid API key" });
                return;
            }

            // valid
            await _next(context);
        }
    }

    public static class ApiKeyAuthenticationExtensions
    {
        public static IApplicationBuilder UseApiKeyAuthentication(this IApplicationBuilder app)
        {
            return app.UseMiddleware<ApiKeyAuthenticationMiddleware>();
        }
    }
}
