using PaymentGateway.Domain.Common.ResponseBase;

namespace PaymentGateway.Domain.Repositories;

public interface IAdminRepository
{
    Task<BaseResultWithData<Object>> ManagePaymentTransactions();
}