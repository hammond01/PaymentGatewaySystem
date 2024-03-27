using IdGen;

namespace PaymentGateway.Domain.Entities;

public class DetailTransaction
{
    public long DetailTransactionId { get; set; }
    public string? DetailTransactionName { get; set; }
    public string? DetailTransactionIpAddress { get; set; }
    public DateTime DetailTransactionCreateAt { get; set; }
    public string? DetailTransactionUserId { get; set; }
    public long PaymentTransactionId { get; set; }
    public long ResponseCodeId { get; set; }
    public string? BankCode { get; set; }

    public static DetailTransaction DetailTransactionGenerator(CreateDetailTransaction request)
    {
        var generator = new IdGenerator(0);
        var detailTransaction = new DetailTransaction
        {
            DetailTransactionId = generator.CreateId(),
            DetailTransactionName = request.DetailTransactionName,
            DetailTransactionIpAddress = request.DetailTransactionIpAddress,
            DetailTransactionCreateAt = DateTime.Now,
            DetailTransactionUserId = request.DetailTransactionUserId,
            PaymentTransactionId = request.TransactionId,
            ResponseCodeId = request.ResponseCodeId,
            BankCode = request.BankCode
        };
        return detailTransaction;
    }
}
public class CreateDetailTransaction
{
    public string? DetailTransactionName { get; set; }
    public string? DetailTransactionIpAddress { get; set; }
    public string? DetailTransactionUserId { get; set; }
    public long TransactionId { get; set; }
    public long ResponseCodeId { get; set; }
    public string? BankCode { get; set; }
}