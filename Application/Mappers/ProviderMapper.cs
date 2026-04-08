using Newtonsoft.Json.Linq;
using PAN.API.Application.DTOs.Common;

namespace PAN.API.Application.Mappers;

public static class ProviderMapper
{
    public static PanCommonResponseDto MapSurePass(string json)
    {
        var j = JObject.Parse(json);

        return new PanCommonResponseDto
        {
            Pan = j["data"]?["pan_number"]?.ToString(),
            FullName = j["data"]?["full_name"]?.ToString(),
            PanStatus = j["data"]?["status"]?.ToString(),
            Category = j["data"]?["category"]?.ToString(),
            AadhaarLinked = j["data"]?["aadhaar_linked"]?.ToObject<bool>() ?? false
        };
    }

    public static PanCommonResponseDto MapSprint(string json)
    {
        var j = JObject.Parse(json);

        return new PanCommonResponseDto
        {
            Pan = j["result"]?["pan"]?.ToString(),
            FullName = j["result"]?["name"]?.ToString(),
            PanStatus = j["result"]?["status"]?.ToString(),
            Category = j["result"]?["type"]?.ToString(),
            AadhaarLinked = j["result"]?["aadhaarLinked"]?.ToObject<bool>() ?? false
        };
    }
}