using PaymentGateway.Domain.Common.ResponseBase;
using static PaymentGateway.Domain.Request.VNPaySandBox.VNPaySanBoxRequest;
namespace PaymentGateway.Domain.Repositories;

public interface IPaymentServices
{
    Task<BaseResult> CreatePaymentAsync(CreatePayment createPayment);
}