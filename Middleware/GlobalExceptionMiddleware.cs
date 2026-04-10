using System.Text;
using PAN.API.Application.DTOs.Response;
using PAN.API.Infrastructure.Logging;

namespace PAN.API.Middleware;

public class GlobalExceptionMiddleware
{
    private readonly RequestDelegate _next;

    public GlobalExceptionMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context)
    {
        var correlationId = context.Items["CorrelationId"]?.ToString();

        try
        {
            // ===== REQUEST =====
            context.Request.EnableBuffering();

            var requestBody = await new StreamReader(
                context.Request.Body,
                Encoding.UTF8,
                leaveOpen: true
            ).ReadToEndAsync();

            context.Request.Body.Position = 0;

            var maskedRequest = MaskingHelper.MaskSensitiveData(requestBody);

            // ===== RESPONSE CAPTURE =====
            var originalBody = context.Response.Body;
            using var newBody = new MemoryStream();
            context.Response.Body = newBody;

            var start = DateTime.UtcNow;

            await _next(context);

            var duration = (DateTime.UtcNow - start).TotalMilliseconds;

            // ===== RESPONSE =====
            newBody.Seek(0, SeekOrigin.Begin);
            var responseBody = await new StreamReader(newBody).ReadToEndAsync();

            var maskedResponse = MaskingHelper.MaskSensitiveData(responseBody);

            newBody.Seek(0, SeekOrigin.Begin);
            await newBody.CopyToAsync(originalBody);

            // ===== LOG REQUEST =====
            SafeLogger.Request($@"
Endpoint : {context.Request.Path}
Method   : {context.Request.Method}
Body     : {maskedRequest}
", correlationId);

            // ===== LOG RESPONSE =====
            SafeLogger.Response($@"
Status   : {context.Response.StatusCode}
Duration : {duration} ms
Body     : {maskedResponse}
", correlationId);
        }
        catch (Exception ex)
        {
            SafeLogger.Error(ex, "Unhandled Exception", context);

            context.Response.StatusCode = 500;

            await context.Response.WriteAsJsonAsync(new ErrorResponse
            {
                Message = ex.Message
            });
        }
    }
}