// Application/DTOs/Provider/SurePassResponseDto.cs
namespace PAN.API.Application.DTOs.Provider;

public class SurePassResponseDto
{
    public string status { get; set; }
    public Data data { get; set; }

    public class Data
    {
        public string pan { get; set; }
        public string fullName { get; set; }
        public string panStatus { get; set; }
        public bool aadhaarLinked { get; set; }
        public string category { get; set; }
    }
}