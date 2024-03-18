namespace PaymentGateway.Domain.Request;

public class PaymentTransactionRequest
{
    public string PaymentTransactionId { get; set; } = string.Empty;
    public string PaymentContent { get; set; } = string.Empty;
    public string PaymentCurrency { get; set; } = string.Empty;
    public decimal PaidAmount { get; set; }
    public string? PaymentLanguage { get; set; } = string.Empty;
    public string? MerchantId { get; set; } = string.Empty;
    public string? PaymentStatus { get; set; } = string.Empty;
    public string? IpAddress { get; set; } = string.Empty;
    public string? PaymentCompletionTime { get; set; } = string.Empty;
}