// Application/DTOs/Response/ApiResponse.cs
namespace PAN.API.Application.DTOs.Response;

public class ApiResponse
{
    public required string Status { get; set; }
    public object Data { get; set; }
}