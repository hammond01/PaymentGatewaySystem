namespace PaymentGateway.Domain.Entities;
public class Payment
{
    public string PaymentId { get; set; } = string.Empty;
    public string PaymentContent { get; set; } = string.Empty;
    public string PaymentCurrency { get; set; } = string.Empty;
    public string PaymentRefId { get; set; } = string.Empty;
    public decimal? RequiredAmount { get; set; }
    public DateTime? PaymentDate { get; set; } = DateTime.Now;
    public DateTime? ExpireDate { get; set; }
    public string? PaymentLanguage { get; set; } = string.Empty;
    public string? MerchantId { get; set; } = string.Empty;
    public string? DestinationId { get; set; } = string.Empty;
    public decimal? PaidAmount { get; set; }
    public string? PaymentStatus { get; set; } = string.Empty;
    public string? PaymentLastMessage { get; set; } = string.Empty;
}
