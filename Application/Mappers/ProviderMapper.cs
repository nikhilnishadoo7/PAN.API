using Newtonsoft.Json.Linq;
using PAN.API.Application.DTOs.Common;

namespace PAN.API.Application.Mappers;

public static class ProviderMapper
{
    public static PanCommonResponseDto MapSurePass(string json)
    {
        var j = JObject.Parse(json);

        var status = j["status"]?.ToString();

        return new PanCommonResponseDto
        {
            IsSuccess = string.Equals(status, "SUCCESS", StringComparison.OrdinalIgnoreCase),

            Pan = j["data"]?["pan_number"]?.ToString(),
            FullName = j["data"]?["full_name"]?.ToString(),
            PanStatus = j["data"]?["pan_status"]?.ToString(),
            Category = j["data"]?["category"]?.ToString(),

            AadhaarLinked = string.Equals(
                j["data"]?["aadhaar_seeding_status"]?.ToString(),
                "Y",
                StringComparison.OrdinalIgnoreCase
            )
        };
    }

    public static PanCommonResponseDto MapSprint(string json)
    {
        var j = JObject.Parse(json);

        var status = j["status"]?.ToString();

        return new PanCommonResponseDto
        {
            IsSuccess = string.Equals(status, "SUCCESS", StringComparison.OrdinalIgnoreCase),

            Pan = j["data"]?["idNumber"]?.ToString(),
            FullName = j["data"]?["fullName"]?.ToString(),
            PanStatus = j["data"]?["panStatus"]?.ToString(),

            Category = "person",

            AadhaarLinked = string.Equals(
                j["data"]?["aadhaarSeedingStatus"]?.ToString(),
                "Successful",
                StringComparison.OrdinalIgnoreCase
            )
        };
    }
}