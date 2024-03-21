using PaymentGateway.Domain.Common.ResponseBase;
using PaymentGateway.Domain.Entities;

namespace PaymentGateway.Domain.Repositories;

public interface IMerchantService
{
    Task<BaseResult> CreateMerchant(CreateMerchantModel createMerchant);
    Task<BaseResultWithData<List<GetMerchantModel>>> GetMerchants();
    Task<BaseResult> UpdateNameMerchant(string merchantId, UpdateNameMerchantModel nameMerchant);
    Task<BaseResult> IsActiveMerchant(string merchantId, IsActiveMerchantModel activeMerchant);
    Task<BaseResult> DeleteMerchant(int id, DeleteMerchantModel deleteMerchant);
}