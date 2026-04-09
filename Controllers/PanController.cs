using Microsoft.AspNetCore.Mvc;
using PAN.API.Application.DTOs.Request;
using PAN.API.Application.Interfaces;

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
        Console.WriteLine("Controller Hit");

        if (request == null)
            return BadRequest("Request body missing");

        Console.WriteLine("PAN: " + request.Pan);

        if (string.IsNullOrWhiteSpace(request.Pan))
            return BadRequest("PAN is empty");

        var correlationId = Guid.NewGuid().ToString();
        var ip = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "UNKNOWN";

        var result = await _service.VerifyAsync(request, correlationId, ip);

        return Ok(result);
    }
}