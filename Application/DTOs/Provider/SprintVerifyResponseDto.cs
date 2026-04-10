// Application/DTOs/Provider/SprintVerifyResponseDto.cs
namespace PAN.API.Application.DTOs.Provider;

public class SprintVerifyResponseDto
{
    public string status { get; set; }
    public Data data { get; set; }

    public class Data
    {
        public string? pan { get; set; }
        public string? fullName { get; set; }
        public string? panStatus { get; set; }
        public bool? aadhaarLinked { get; set; }
        public string?   category { get; set; }
    }
}