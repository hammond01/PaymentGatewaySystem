using PaymentGateway.Domain.Common.ResponseBase;
using PaymentGateway.Domain.Request;

namespace PaymentGateway.Domain.Repositories.VNPayRestful;

public interface IVnPayServices
{
    public Task<BaseResult> CreateQrString(CreateQrRequest createQrRequest);
}