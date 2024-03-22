namespace PaymentGateway.Domain.Entities.ThirdParty.VNPayEntities;

public class VNPaySandBoxEntities
{
    public string? Version { get; set; }
    public string? Command { get; set; }
    public string? TmnCode { get; set; }
    public string? Locale { get; set; }
    public string? CurrCode { get; set; }
    public string? ReturnUrl { get; set; }
    public string? Amount { get; set; }
    public string? CreateDate { get; set; }
    public string? IpAddr { get; set; }
    public string? OrderInfo { get; set; }
    public string? OrderType { get; set; }
    public string? TxnRef { get; set; }
    public string? SecureHash { get; set; }
    public string? BankCode { get; set; }
    public string? BankTranNo { get; set; }
    public string? PayDate { get; set; }
    public string? CardType { get; set; }
    public string? TransactionNo { get; set; }
    public string? ResponseCode { get; set; }
    public string? TransactionStatus { get; set; }
    public string? SecureHashType { get; set; }
}

public class CreatePaymentURLRequestTransfer
{
    public string? Version { get; set; }
    public string? Command { get; set; }
    public string? TmnCode { get; set; }
    public string? Amount { get; set; }
    public string? BankCode { get; set; }
    public string? BaseUrl { get; set; }
    public string? HashSecret { get; set; }
    public string? CurrCode { get; set; }
    public string? CreateDate { get; set; }
    public string? IpAddr { get; set; }
    public string? Locale { get; set; }
    public string? OrderInfo { get; set; }
    public string? OrderType { get; set; }
    public string? ReturnUrl { get; set; }
    public long TxnRef { get; set; }
}

public class CreateStringUrlResponse
{
    public string? TmnCode { get; set; }
    public long Amount { get; set; }
    public string? BankCode { get; set; }
    public string? BankTranNo { get; set; }
    public string? CardType { get; set; }
    public string? PayDate { get; set; }
    public string? OrderInfo { get; set; }
    public string? TransactionNo { get; set; }
    public string? ResponseCode { get; set; }
    public string? TransactionStatus { get; set; }
    public long TxnRef { get; set; }
    public string? SecureHashType { get; set; }
    public string? SecureHash { get; set; }


}
public class CreateStringUrlRequest
{
    public int OrderId { get; set; }
    public string? PaymentContent { get; set; }
    public decimal Amount { get; set; }
    public long MerchantId { get; set; }
    public string? Channel { get; set; }
    public string? ClientName { get; set; }
    public string? LastMessage { get; set; }
}

public class RefundRequest
{
    public string? RequestId { get; set; }
    public string? Version { get; set; }
    public string? Command { get; set; }
    public string? TmnCode { get; set; }
    public string? TransactionType { get; set; }
    public string? TxnRef { get; set; }
    public long Amount { get; set; }
    public string? OrderInfo { get; set; }
    public string? TransactionNo { get; set; }
    public string? TransactionDate { get; set; }
    public string? CreateBy { get; set; }
    public string? CreateDate { get; set; }
    public string? IpAddr { get; set; }
    public string? SecureHash { get; set; }
}

public class RefundRequestClient
{
    public string? TxnRef { get; set; }
    public string? Amount { get; set; }
    public string? TransactionDate { get; set; }
    public string? TransactionType { get; set; }
    public string? CreateBy { get; set; }
    public string? TransactionNo { get; set; }
}
public class RefundResponse
{
    public string? ResponseId { get; set; }
    public string? Command { get; set; }
    public string? TmnCode { get; set; }
    public string? TxnRef { get; set; }
    public string? Amount { get; set; }
    public string? OrderInfo { get; set; }
    public string? ResponseCode { get; set; }
    public string? Message { get; set; }
    public string? BankCode { get; set; }
    public string? PayDate { get; set; }
    public string? TransactionNo { get; set; }
    public string? TransactionType { get; set; }
    public string? TransactionStatus { get; set; }
    public string? SecureHash { get; set; }

}