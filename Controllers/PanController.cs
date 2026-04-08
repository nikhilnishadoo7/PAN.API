using Microsoft.AspNetCore.Mvc;
using PAN.API.Application.Interfaces;
using PAN.API.Application.DTOs.Request;
using System;
using System.Threading.Tasks;

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
    public async Task<IActionResult> Verify([FromBody] PanRequest request)
    {
        try
        {
            if (request == null || string.IsNullOrWhiteSpace(request.Pan))
                return BadRequest("PAN is required");

            var correlationId = Guid.NewGuid().ToString();
            var ip = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "UNKNOWN";

            var result = await _service.VerifyAsync(request, correlationId, ip);

            return Ok(result);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = ex.Message });
        }
    }
}