using PaymentGateway.Domain.Common.ResponseBase;
using PaymentGateway.Domain.Entities;
using PaymentGateway.Domain.Request;

namespace PaymentGateway.Domain.Repositories;

public interface IMerchantServices
{
    Task<BaseResult> CreateMerchant(Merchant merchant);
    Task<BaseResult> GetMerchants();
    Task<CommandResponse> UpdateNameMerchant(string merchantId, MerchantRequest.UpdateNameMerchant nameMerchant);
    Task<CommandResponse> IsActiveMerchant(string merchantId, MerchantRequest.IsActiveMerchant activeMerchant);
}