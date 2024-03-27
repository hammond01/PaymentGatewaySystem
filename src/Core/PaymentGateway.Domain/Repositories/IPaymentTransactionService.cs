using PaymentGateway.Domain.Common.ResponseBase;
using PaymentGateway.Domain.Entities;

namespace PaymentGateway.Domain.Repositories;

public interface IPaymentTransactionService
{
    Task<BaseResultWithData<long>> CreatePaymentTransactionAsync(CreatePaymentTransactionModel paymentTransactionRequest);
    Task<BaseResult> UpdatePaymentTransactionAsync(long updateId, PaymentCompletion paymentCompletion);
    Task<BaseResultWithData<CheckTransactionStatus>> CheckTransactionStatus(long transactionId);
}