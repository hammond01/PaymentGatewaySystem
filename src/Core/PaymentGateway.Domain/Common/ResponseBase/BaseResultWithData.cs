namespace PaymentGateway.Domain.Common.ResponseBase;

public class BaseResultWithData<T> : BaseResult
{
    public T? Data { get; set; }
}