using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace PAN.API.Application.DTOs.Request;

public class PanRequest
{
    [JsonPropertyName("pan_number")]
    [Required]
    public string Pan { get; set; }
}