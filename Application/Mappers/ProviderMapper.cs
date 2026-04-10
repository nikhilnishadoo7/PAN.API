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
            IsSuccess = j["status"]?.ToString() == "SUCCESS",
            Pan = j["data"]?["pan_number"]?.ToString(),
            FullName = j["data"]?["full_name"]?.ToString(),
            PanStatus = j["data"]?["pan_status"]?.ToString(),
            Category = j["data"]?["category"]?.ToString(),
            AadhaarLinked = j["data"]?["aadhaar_seeding_status"]?.ToString() == "Y",
            //requestId = j["data"]?["requestId"]?.ToString(),
            client_id = j["data"]?["client_id"]?.ToString(),
        };
    }

    public static PanCommonResponseDto MapSprint(string json)
    {
        var j = JObject.Parse(json);

        return new PanCommonResponseDto
        {
            IsSuccess = j["status"]?.ToString() == "SUCCESS",
            Pan = j["data"]?["idNumber"]?.ToString(),
            FullName = j["data"]?["fullName"]?.ToString(),
            PanStatus = j["data"]?["panStatus"]?.ToString(),
            Category = "person",
            AadhaarLinked = j["data"]?["aadhaarSeedingStatus"]?.ToString() == "Successful",
            client_id = j["requestId"].ToString()
        };
    }
}