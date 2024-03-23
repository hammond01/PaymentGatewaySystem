namespace PaymentGateway.Domain.Entities.ThirdParty.VNPayEntities;

public class DetailPaymentVNPSandBox
{
    public long DetailPaymentId { get; set; }
    public long PaymentTransactionId { get; set; }
    public string? TmnCode { get; set; }
    public string? BankTranNo { get; set; }
    public long Amount { get; set; }
    public string? BankCode { get; set; }
    public string? CardType { get; set; }
    public string? PayDate { get; set; } //yyyyMMddHHmmss
    public string? OrderInfo { get; set; }
    public long TransactionNo { get; set; }
    public string? ResponseCode { get; set; }
    public string? TransactionStatus { get; set; }
    public long TxnRef { get; set; }
    public string? SecureHashType { get; set; }
    public string? SecureHash { get; set; }
    public string? JsonData { get; set; }
}