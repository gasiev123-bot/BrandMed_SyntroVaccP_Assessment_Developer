using Microsoft.Extensions.Configuration;

namespace SyntroVaccPApp.Middleware
{
    public class ApiKeyMiddleware
    {
        private readonly RequestDelegate _next;
        private const string APIKEYNAME = "X-API-KEY";

        public ApiKeyMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, IConfiguration config)
        {
            if (context.Request.Path.StartsWithSegments("/api"))
            {
                if (!context.Request.Headers.TryGetValue(APIKEYNAME, out var extractedApiKey))
                {
                    context.Response.StatusCode = 401;
                    await context.Response.WriteAsync("API Key missing");
                    return;
                }

                var apiKey = config["ApiSettings:ApiKey"];

                if (string.IsNullOrEmpty(apiKey) || !apiKey.Equals(extractedApiKey))
                {
                    context.Response.StatusCode = 401;
                    await context.Response.WriteAsync("Invalid API Key");
                    return;
                }

            }

            await _next(context);
        }
    }
}
