namespace PAN.API.Domain.Entities;

public class PanResponseJson
{
    public Guid Id { get; set; }
    public string CorrelationId { get; set; }
    public Guid PanVerificationId { get; set; }

    public string EncryptedRawResponseJson { get; set; }
    public DateTime CreatedAt { get; set; }
}