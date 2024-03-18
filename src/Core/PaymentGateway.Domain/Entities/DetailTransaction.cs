using PaymentGateway.Domain.Request;

namespace PaymentGateway.Domain.Entities;

public class DetailTransaction
{
    public string DetailTransactionId { get; set; } = string.Empty;
    public string DetailTransactionName { get; set; } = string.Empty;
    public string DetailTransactionIpAddress { get; set; } = string.Empty;
    public DateTime DetailTransactionCreateAt { get; set; } = DateTime.Now;
    public string DetailTransactionUserId { get; set; } = string.Empty;
    public string PaymentTransactionId { get; set; } = string.Empty;
    public string ReponseCodeId { get; set; } = string.Empty;
    public string BankCode { get; set; } = string.Empty;

    public static DetailTransaction DetailTransactionGenerator(DetailTransactionRequest request)
    {
        var detailTransaction = new DetailTransaction
        {
            DetailTransactionId = Guid.NewGuid().ToString(),
            DetailTransactionName = request.DetailTransactionName,
            DetailTransactionIpAddress = request.DetailTransactionIpAddress,
            DetailTransactionCreateAt = DateTime.Now,
            DetailTransactionUserId = request.DetailTransactionUserId,
            PaymentTransactionId = request.TransactionId,
            ReponseCodeId = request.ReponseCodeId,
            BankCode = request.BankCode
        };
        return detailTransaction;
    }
}