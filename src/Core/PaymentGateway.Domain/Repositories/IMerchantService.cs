using PaymentGateway.Domain.Common.ResponseBase;
using PaymentGateway.Domain.Entities;

namespace PaymentGateway.Domain.Repositories;

public interface IMerchantService
{
    Task<BaseResult> CreateMerchant(CreateMerchantModel createMerchant);
    Task<BaseResultWithData<List<GetMerchantModel>>> GetMerchants();
    Task<BaseResult> UpdateNameMerchant(long merchantId, UpdateNameMerchantModel nameMerchant);
    Task<BaseResult> IsActiveMerchant(long merchantId, IsActiveMerchantModel activeMerchant);
    Task<BaseResult> DeleteMerchant(long merchantId);
}