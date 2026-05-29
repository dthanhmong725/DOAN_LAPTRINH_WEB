namespace DOAN_LAPTRINHWEB.Middleware;

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
        var ipAddress = GetClientIpAddress(context);
        var userAgent = context.Request.Headers.UserAgent.ToString();

        context.Items["ClientIpAddress"] = ipAddress;
        context.Items["ClientUserAgent"] = userAgent;

        await _next(context);

        var statusCode = context.Response.StatusCode;
        if (statusCode >= 400)
        {
            _logger.LogWarning("HTTP {Method} {Path} responded {StatusCode} from IP {IpAddress}",
                context.Request.Method, context.Request.Path, statusCode, ipAddress);
        }
    }

    private static string? GetClientIpAddress(HttpContext context)
    {
        // X-Forwarded-For: used by reverse proxies (nginx, cloudflare, etc.)
        var forwardedFor = context.Request.Headers["X-Forwarded-For"].FirstOrDefault();
        if (!string.IsNullOrEmpty(forwardedFor))
        {
            // First IP in the list is the original client
            var ip = forwardedFor.Split(',')[0].Trim();
            if (!string.IsNullOrEmpty(ip)) return ip;
        }

        // X-Real-IP: used by some proxies
        var realIp = context.Request.Headers["X-Real-IP"].FirstOrDefault();
        if (!string.IsNullOrEmpty(realIp))
            return realIp;

        // CF-Connecting-IP: Cloudflare
        var cfIp = context.Request.Headers["CF-Connecting-IP"].FirstOrDefault();
        if (!string.IsNullOrEmpty(cfIp))
            return cfIp;

        // Fallback to remote IP address
        return context.Connection.RemoteIpAddress?.ToString();
    }
}

public static class RequestLoggingMiddlewareExtensions
{
    public static IApplicationBuilder UseRequestLogging(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<RequestLoggingMiddleware>();
    }
}
