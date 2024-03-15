using PaymentGateway.Domain.Common.ResponseBase;
using PaymentGateway.Domain.Repositories;
using PaymentGateway.Ultils.ConfigDBConnection.Impl;
using PaymentGateway.Ultils.Extension;
using static PaymentGateway.Domain.Request.VNPaySandBox.VNPaySanBoxRequest;
namespace PaymentGateway.Persistence.Repositories;

public class PaymentServices : IPaymentServices
{
    private readonly IDataAccess _db;

    public PaymentServices(IDataAccess db)
    {
        _db = db;
    }
    public async Task<BaseResult> CreatePaymentAsync(CreatePayment createPayment)
    {
        var query = Extensions.GetInsertQuery("Payment", "PaymentId", "PaymentId", "PaymentContent", "PaymentCurrency",
                       "PaymentDate", "ExpireDate", "PaymentLanguage", "MerchantId", "PaidAmount", "PaymentStatus");
        createPayment.PaymentId = "P" + Guid.NewGuid();
        var result = await _db.SaveData(query, createPayment);
        if (result)
            return new BaseResult
            { IsSuccess = true, Message = "Create new payment success", StatusCode = 200 };
        return new BaseResult { IsSuccess = false, Message = "Adding a new record failed!", StatusCode = 404 };
    }
}