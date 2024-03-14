namespace PaymentGateway.Domain.Common.ResponseBase;

public class CommandResponse
{
    public bool IsSuccess { get; set; }
    public string? Message { get; set; } = string.Empty;
    public int StatusCode { get; set; }
}