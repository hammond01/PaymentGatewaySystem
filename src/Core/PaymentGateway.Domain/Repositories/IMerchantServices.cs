using PaymentGateway.Domain.Common.ResponseBase;
using static PaymentGateway.Domain.Request.MerchantRequest;

namespace PaymentGateway.Domain.Repositories;

public interface IMerchantServices
{
    Task<BaseResult> CreateMerchant(CreateMerchant createMerchant);
    Task<BaseResult> GetMerchants();
    Task<CommandResponse> UpdateNameMerchant(string merchantId, UpdateNameMerchant nameMerchant);
    Task<CommandResponse> IsActiveMerchant(string merchantId, IsActiveMerchant activeMerchant);
}