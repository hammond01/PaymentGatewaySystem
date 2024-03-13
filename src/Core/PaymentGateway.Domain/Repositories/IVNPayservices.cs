using PaymentGateway.Domain.Common.ResponseBase;
using PaymentGateway.Domain.Request;

namespace PaymentGateway.Domain.Repositories;
public interface IVNPayservices
{
    public Task<BaseResult> CreateQRString(CreateQRRequest createQRRequest);
}
