// Utilities/ApiResponseBuilder.cs
using PAN.API.Application.DTOs.Response;

namespace PAN.API.Utilities;

public static class ApiResponseBuilder
{
    public static ApiResponse Success(object data)
    {
        return new ApiResponse
        {
            Status = "SUCCESS",
            Data = data
        };
    }
}