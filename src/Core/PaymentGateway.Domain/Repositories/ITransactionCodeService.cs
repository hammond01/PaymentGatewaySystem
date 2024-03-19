using PaymentGateway.Domain.Entities;

namespace PaymentGateway.Domain.Repositories;

public interface ITransactionCodeService
{
    Task<List<TransactionCode>> GetTransactionCodeByTypeAsync(string typeName);
    Task<TransactionCode> GetTransactionCodeByCodeAsync(string code, string requestTypeName);
}