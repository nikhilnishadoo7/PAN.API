using Serilog.Context;

namespace PAN.API.Middleware;

public class CorrelationIdMiddleware
{
    private readonly RequestDelegate _next;

    public CorrelationIdMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context)
    {
        var id = context.Request.Headers["X-Request-Id"].FirstOrDefault()
                 ?? Guid.NewGuid().ToString();

        // ✅ Store in HttpContext
        context.Items["CorrelationId"] = id;

        // ✅ Send back to client
        context.Response.Headers["X-Request-Id"] = id;

        // ❗ THIS LINE WAS MISSING (ROOT CAUSE)
        using (LogContext.PushProperty("correlation_id", id))
        {
            await _next(context);
        }
    }
}