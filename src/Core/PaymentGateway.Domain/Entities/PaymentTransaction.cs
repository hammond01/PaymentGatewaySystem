namespace PaymentGateway.Domain.Entities;

public class PaymentTransaction
{
    public string TransactionId { get; set; } = string.Empty;
    public string? TransactionMessage { get; set; } = string.Empty;
    public string? TransactionPayload { get; set; } = string.Empty;
    public string? TransactionStatus { get; set; } = string.Empty;
    public decimal? TransactionAmount { get; set; }
    public DateTime? TransactionDate { get; set; }
    public string? PaymentId { get; set; } = string.Empty;
}