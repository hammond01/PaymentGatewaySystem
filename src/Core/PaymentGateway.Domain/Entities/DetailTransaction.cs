namespace PaymentGateway.Domain.Entities;

public class DetailTransaction
{
    public string DetailTransactionId { get; set; } = string.Empty;
    public string DetailTransactionName { get; set; } = string.Empty;
    public string DetailTransactionAcction { get; set; } = string.Empty;
    public string DetailTransactionController { get; set; } = string.Empty;
    public string DetailTransactionIpAddress { get; set; } = string.Empty;
    public DateTime DetailTransactionCreateAt { get; set; } = DateTime.Now;
    public string DetailTransactionUserId { get; set; } = string.Empty;
    public string TransactionId { get; set; } = string.Empty;
}