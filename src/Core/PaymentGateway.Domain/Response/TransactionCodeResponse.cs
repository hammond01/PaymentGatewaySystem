namespace PaymentGateway.Domain.Response;

public class TransactionCodeResponse
{
    public string? ReponseCodeId { get; set; }
    public string? RequestTypeId { get; set; }
    public string? ResponseCode { get; set; }
    public string? Message { get; set; }
    public string? RequestName { get; set; }
}