namespace PaymentGateway.Domain.Request;

public class CreateQrRequest
{
    public string productId { get; set; } = string.Empty;
    public string amount { get; set; } = string.Empty;
    public string desc { get; set; } = string.Empty;
    public string billNumber { get; set; } = string.Empty;
    public string tipAndFee { get; set; } = string.Empty;
    //Mã đơn hàng
    public string txnId { get; set; } = string.Empty;
}