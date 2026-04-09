namespace PAN.API.Domain.Entities;

public class PanResponseJson
{
    public long Id { get; set; }   // ✅ FIXED

    public string CorrelationId { get; set; }

    public Guid PanVerificationId { get; set; }

    public string EncryptedRawResponseJson { get; set; }

    public DateTime CreatedAt { get; set; }
}