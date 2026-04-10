using Microsoft.AspNetCore.Mvc;
using PAN.API.Application.DTOs.Request;
using PAN.API.Application.Services.Interfaces;
using PAN.API.Infrastructure.Logging;

namespace PAN.API.Controllers;

[ApiController]
[Route("api/v1/pan")]
public class PanController : ControllerBase
{
    private readonly IPanVerificationService _service;

    public PanController(IPanVerificationService service)
    {
        _service = service;
    }

    [HttpPost("verify")]
    public async Task<IActionResult> Verify([FromBody] PanRequest? request)
    {
        SafeLogger.App("Controller Hit");

        if (request == null)
            return BadRequest("Request body missing");

        SafeLogger.App($"Incoming PAN: {request.Pan}");

        if (string.IsNullOrWhiteSpace(request.Pan))
            return BadRequest("PAN is empty");

        // ✅ FIXED
        var correlationId = HttpContext.Items["CorrelationId"]?.ToString();

        var ip = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "UNKNOWN";

        var result = await _service.VerifyAsync(request, correlationId, ip);

        SafeLogger.App("Returning response");

        return Ok(result);
    }
}