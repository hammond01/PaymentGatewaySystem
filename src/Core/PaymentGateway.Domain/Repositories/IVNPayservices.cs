using PaymentGateway.Domain.Common.ResponseBase;
using PaymentGateway.Domain.Request;

namespace PaymentGateway.Domain.Repositories;

public interface IVnPayServices
{
    public Task<BaseResult> CreateQrString(CreateQrRequest createQrRequest);
}