namespace PaymentGateway.Domain.Entities;
public class PaymentSignature
{
    public string SignatureId { get; set; } = string.Empty;
    public string? PaymentId { get; set; } = string.Empty;
    public string? SignatureValue { get; set; } = string.Empty;
    public string? SignatureAIGo { get; set; } = string.Empty;
    public string? SignatureOwn { get; set; } = string.Empty;
    public DateTime? SignatureDate { get; set; }
    public bool IsValid { get; set; }
}
