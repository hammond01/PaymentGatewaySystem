namespace PaymentGateway.Domain.Request;

public class DetailTransactionRequest
{
    public string DetailTransactionName { get; set; } = string.Empty;
    public string DetailTransactionIpAddress { get; set; } = string.Empty;
    public string DetailTransactionUserId { get; set; } = string.Empty;
    public string TransactionId { get; set; } = string.Empty;
    public string ReponseCodeId { get; set; } = string.Empty;
    public string BankCode { get; set; } = string.Empty;
}