using PaymentGateway.Domain.Common.ResponseModel;
using PaymentGateway.Domain.Entities.ThirdParty;

namespace PaymentGateway.Domain.Repositories;
public interface IVNPayservices
{
    public Task<BaseResult> CreateQRString(CreateQR createQR);
}
