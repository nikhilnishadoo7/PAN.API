// Application/DTOs/Request/PanRequest.cs
using System.Text.Json.Serialization;

namespace PAN.API.Application.DTOs.Request;

public class PanRequest
{
    [JsonPropertyName("pan")]
    public string Pan { get; set; }
}