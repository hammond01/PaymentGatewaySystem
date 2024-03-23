namespace PaymentGateway.Domain.Entities;

public class DetailPayment
{
    public long DetailPaymentId { get; set; }
    public long PaymentTransactionId { get; set; }
    public string? TmnCode { get; set; }
    public string? Amount { get; set; }
    public string? BankCode { get; set; }
    public string? TmnCoBankTranNode { get; set; }
    public string? CardType { get; set; }
    public string? PayDate { get; set; } //yyyyMMddHHmmss
    public string? OrderInfo { get; set; }
    public string? TransactionNo { get; set; }
    public string? ResponseCode { get; set; }
    public string? TransactionStatus { get; set; }
    public string? TxnRef { get; set; }
    public string? SecureHashType { get; set; }
    public string? SecureHash { get; set; }
    public string? Apptransid { get; set; }
    public string? Servertime { get; set; }
    public string? UserFeeAmount { get; set; }
    public string? DiscountAmount { get; set; }
    public string? JsonData { get; set; }
    public string? StringQR { get; set; }
}