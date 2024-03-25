using Microsoft.AspNetCore.Http;
using PaymentGateway.Domain.Common.ResponseBase;
using PaymentGateway.Domain.Constants;
using PaymentGateway.Domain.Entities.ThirdParty.VNPayEntities;
using PaymentGateway.Domain.Exceptions.ErrorMessage;
using PaymentGateway.Domain.Repositories;
using PaymentGateway.Ultils.ConfigDBConnection.Impl;
using Serilog;

namespace PaymentGateway.Persistence.Repositories;

public class DetailPaymentRepository : IDetailPaymentService
{
    private readonly IDataAccess _db;

    public DetailPaymentRepository(IDataAccess db)
    {
        _db = db;
    }

    public async Task<BaseResult> CreateDataToDetailPaymentAsync(DetailPaymentVNPSandBox request)
    {
        try
        {
            var result = await _db.InsertData("DetailPayment", request);
            return new BaseResult
            {
                IsSuccess = result,
                StatusCode = StatusCodes.Status201Created,
                Message = MessageConstantsWithValue.createSuccess("detail payment")
            };
        }
        catch (Exception e)
        {
            Log.Error(LayerErrorMessage.ERROR_AT_PERSISTENCE(e.Message));
            throw;
        }
    }
}