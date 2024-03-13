namespace PaymentGateway.Domain.Common.ResponseBase;
public class BaseResult
{
    public bool IsSuccess { get; set; }
    public string? Message { get; set; } = string.Empty;
    public object Data { get; set; } = default!;
    public int StatusCode { get; set; }
    public void Set(bool success, string message)
    {
        this.IsSuccess = success;
        this.Message = message;
    }
}
