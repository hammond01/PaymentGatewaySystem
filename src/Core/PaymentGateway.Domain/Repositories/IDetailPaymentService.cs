using PaymentGateway.Domain.Common.ResponseBase;
using PaymentGateway.Domain.Entities.ThirdParty.VNPayEntities;

namespace PaymentGateway.Domain.Repositories;

public interface IDetailPaymentService
{
    Task<BaseResult> CreateDataToDetailPaymentAsync(CreateStringUrlResponse createStringUrlResponse);
}