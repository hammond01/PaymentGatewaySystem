using PaymentGateway.Domain.Common.ResponseBase;
using PaymentGateway.Domain.Entities.ThirdParty.VNPayEntities;

namespace PaymentGateway.Domain.Repositories;

public interface ITransactionRefundRepository
{
    Task<BaseResult> InsertDataToRefundTable(RefundResponse model);
}