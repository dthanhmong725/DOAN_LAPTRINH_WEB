using System.Net;
using System.Text.Json;
using DOAN_LAPTRINHWEB.Models.DTOs;
using DOAN_LAPTRINHWEB.Interfaces;

namespace DOAN_LAPTRINHWEB.Middleware;

public class GlobalExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionMiddleware> _logger;
    private readonly IHostEnvironment _env;

    public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger, IHostEnvironment env)
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
            _logger.LogError(ex, "An unhandled exception occurred");
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

        var response = new ApiResponse<object>
        {
            Success = false,
            Message = "An unexpected error occurred",
            Errors = _env.IsDevelopment() ? new List<string> { exception.Message, exception.StackTrace ?? "" } : null
        };

        var options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        var json = JsonSerializer.Serialize(response, options);
        await context.Response.WriteAsync(json);
    }
}

public class RateLimitMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RateLimitMiddleware> _logger;

    public RateLimitMiddleware(RequestDelegate next, ILogger<RateLimitMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context, IRateLimitService rateLimitService)
    {
        // Skip rate limiting for GET requests and static files
        if (context.Request.Method == "GET" || context.Request.Path.StartsWithSegments("/css") ||
            context.Request.Path.StartsWithSegments("/js") || context.Request.Path.StartsWithSegments("/lib"))
        {
            await _next(context);
            return;
        }

        // Get user identifier (IP for anonymous, user ID for authenticated)
        var userId = context.User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value
                    ?? context.Connection.RemoteIpAddress?.ToString()
                    ?? "unknown";

        var endpoint = context.Request.Path.Value ?? "/";

        // Skip rate limiting for auth endpoints (they have their own limits)
        if (endpoint.StartsWith("/api/auth"))
        {
            await _next(context);
            return;
        }

        if (await rateLimitService.IsRateLimitedAsync(userId, endpoint))
        {
            _logger.LogWarning("Rate limit exceeded for user {UserId} on {Endpoint}", userId, endpoint);

            context.Response.StatusCode = (int)HttpStatusCode.TooManyRequests;
            context.Response.Headers["Retry-After"] = "60";

            var response = new ApiResponse<object>
            {
                Success = false,
                Message = "Too many requests. Please try again later."
            };

            context.Response.ContentType = "application/json";
            await context.Response.WriteAsync(JsonSerializer.Serialize(response));
            return;
        }

        await rateLimitService.IncrementAsync(userId, endpoint);
        await _next(context);
    }
}

public static class MiddlewareExtensions
{
    public static IApplicationBuilder UseGlobalExceptionHandler(this IApplicationBuilder app)
    {
        return app.UseMiddleware<GlobalExceptionMiddleware>();
    }

    public static IApplicationBuilder UseRateLimiting(this IApplicationBuilder app)
    {
        return app.UseMiddleware<RateLimitMiddleware>();
    }
}
