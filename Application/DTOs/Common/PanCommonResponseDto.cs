namespace PAN.API.Application.DTOs.Common;

public class PanCommonResponseDto
{
    public bool IsSuccess { get; set; }

    public string? Pan { get; set; }

    public string? FullName { get; set; }

    public string? PanStatus { get; set; }

    public bool AadhaarLinked { get; set; }

    public string? Category { get; set; }
    //public bool? requestId { get; set; }

    public string client_id { get; set; }
}