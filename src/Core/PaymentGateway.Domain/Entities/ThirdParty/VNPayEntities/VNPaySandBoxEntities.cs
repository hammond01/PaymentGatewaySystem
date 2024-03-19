namespace PaymentGateway.Domain.Entities.ThirdParty.VNPayEntities;

public class VNPaySandBoxEntities
{
    public string Version { get; set; } = string.Empty;
    public string Command { get; set; } = string.Empty;
    public string TmnCode { get; set; } = string.Empty;
    public string Locale { get; set; } = string.Empty;
    public string CurrCode { get; set; } = string.Empty;
    public string ReturnUrl { get; set; } = string.Empty;
    public string Amount { get; set; } = string.Empty;
    public string CreateDate { get; set; } = string.Empty;
    public string IpAddr { get; set; } = string.Empty;
    public string OrderInfo { get; set; } = string.Empty;
    public string OrderType { get; set; } = string.Empty;
    public string TxnRef { get; set; } = string.Empty;
    public string SecureHash { get; set; } = string.Empty;
    public string BankCode { get; set; } = string.Empty;
    public string BankTranNo { get; set; } = string.Empty;
    public string PayDate { get; set; } = string.Empty;
    public string CardType { get; set; } = string.Empty;
    public string TransactionNo { get; set; } = string.Empty;
    public string ResponseCode { get; set; } = string.Empty;
    public string TransactionStatus { get; set; } = string.Empty;
    public string SecureHashType { get; set; } = string.Empty;
}

public class CreatePaymentURLRequestTransfer
{
    public string Version { get; set; } = string.Empty;
    public string Command { get; set; } = string.Empty;
    public string TmnCode { get; set; } = string.Empty;
    public string Amount { get; set; } = string.Empty;
    public string BankCode { get; set; } = string.Empty;
    public string BaseUrl { get; set; } = string.Empty;
    public string HashSecret { get; set; } = string.Empty;
    public string CurrCode { get; set; } = string.Empty;
    public string CreateDate { get; set; } = string.Empty;
    public string IpAddr { get; set; } = string.Empty;
    public string Locale { get; set; } = string.Empty;
    public string OrderInfo { get; set; } = string.Empty;
    public string OrderType { get; set; } = string.Empty;
    public string ReturnUrl { get; set; } = string.Empty;
    public string TxnRef { get; set; } = string.Empty;
}

public class CreateStringUrlResponse
{
    public string TmnCode { get; set; } = string.Empty;
    public long Amount { get; set; }
    public string BankCode { get; set; } = string.Empty;
    public string BankTranNo { get; set; } = string.Empty;
    public string CardType { get; set; } = string.Empty;
    public string PayDate { get; set; } = string.Empty;
    public string OrderInfo { get; set; } = string.Empty;
    public string TransactionNo { get; set; } = string.Empty;
    public string ResponseCode { get; set; } = string.Empty;
    public string TransactionStatus { get; set; } = string.Empty;
    public string TxnRef { get; set; } = string.Empty;
    public string SecureHashType { get; set; } = string.Empty;
    public string SecureHash { get; set; } = string.Empty;


}
public class CreateStringUrlRequest
{
    public int OrderId { get; set; }
    public string PaymentContent { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string? MerchantId { get; set; } = string.Empty;

}