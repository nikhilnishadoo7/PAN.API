using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace PAN.API.Application.DTOs.Request;

public class PanRequest
{
    [JsonPropertyName("pan_number")]
    public string? PanNumber { get; set; }

    [JsonPropertyName("idNumber")]
    public string? IdNumber { get; set; }

    [JsonIgnore]
    public string Pan => PanNumber ?? IdNumber!;
}