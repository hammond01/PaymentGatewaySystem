using PaymentGateway.Domain.Common.ResponseBase;
using PaymentGateway.Domain.Entities;
using PaymentGateway.Domain.Response;

namespace PaymentGateway.Domain.Repositories;

public interface IDetailTransactionServices
{
    Task<BaseResult> InsertDetailTransaction(CreateDetailTransaction detailTransactionRequest);
}