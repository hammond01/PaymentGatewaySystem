namespace PaymentGateway.Domain.Common.ResponseModel;
public class BaseError
{
    public string Code { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
}
