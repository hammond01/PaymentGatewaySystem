using PaymentGateway.Domain.Common.ResponseBase;
using PaymentGateway.Domain.Entities.ThirdParty.VNPayEntities;

namespace PaymentGateway.Domain.Repositories;

public interface IDetailPaymentService
{
    Task<BaseResult> CreateDataToDetailPaymentAsync(DetailPaymentVNPSandBox createStringUrlResponse);
    Task<BaseResultWithData<RefundRequestInfo>> GetTransactionInfoForRefundRequest(long transactionNo);
}