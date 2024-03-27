namespace PaymentGateway.Domain.Entities;

//in database, this class is a table responseCode
public class TransactionCode
{
    public long ResponseCodeId { get; set; }
    public string? RequestTypeId { get; set; }
    public string? ResponseCode { get; set; }
    public string? Message { get; set; }
    public string? RequestName { get; set; }
}