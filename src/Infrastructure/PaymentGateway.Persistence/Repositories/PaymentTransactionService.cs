using Microsoft.AspNetCore.Http;
using PaymentGateway.Domain.Common.ResponseBase;
using PaymentGateway.Domain.Constants;
using PaymentGateway.Domain.Entities;
using PaymentGateway.Domain.Repositories;
using PaymentGateway.Domain.Request;
using PaymentGateway.Domain.Response.VNPaySandBox;
using PaymentGateway.Ultils.ConfigDBConnection.Impl;
using PaymentGateway.Ultils.Extension;

namespace PaymentGateway.Persistence.Repositories;

public class PaymentTransactionService : IPaymentTransactionService
{
    private readonly IDataAccess _db;
    private readonly IDetailTransactionServices _detailTransactionServices;
    private readonly Helpers _helpers;
    private readonly ITransactionCodeService _transactionCodeService;

    public PaymentTransactionService(IDataAccess db, IDetailTransactionServices detailTransactionServices,
        Helpers helpers, ITransactionCodeService transactionCodeService)
    {
        _db = db;
        _detailTransactionServices = detailTransactionServices;
        _helpers = helpers;
        _transactionCodeService = transactionCodeService;
    }

    public async Task<BaseResultWithData<CreatePaymentResponse>> CreatePaymentTransactionAsync(PaymentTransactionRequest paymentTransactionRequest)
    {
        try
        {
            var query = Extensions.GetInsertQuery("PaymentTransaction", "PaymentTransactionId",
                "PaymentContent", "PaymentCurrency", "PaymentDate",
                "ExpireDate", "PaymentLanguage", "MerchantId", "PaidAmount", "PaymentStatus", "PaymentLastMessage", "PaymentCompletionTime");
            var paymentTransactionModel = PaymentTransaction.GeneratePaymentTransaction(paymentTransactionRequest);
            var result = await _db.SaveData(query, paymentTransactionModel);
            //create detail transaction
            var paymentStatus =
                await _transactionCodeService.GetTransactionCodeByTypeAsync(RequestTypeTransactionConstants
                    .COMMON_TRANSACTION);
            var detailTransactionRequest = new DetailTransactionRequest
            {
                DetailTransactionName = "Create new payment transaction",
                DetailTransactionIpAddress = paymentTransactionRequest.IpAddress!,
                DetailTransactionUserId = "User Id example",
                TransactionId = paymentTransactionModel.PaymentTransactionId,
                ReponseCodeId = paymentStatus
                    .FirstOrDefault(x => x.ResponseCode!.Trim() == ResponseCodeConstants.AWAITING_PAYMENT)?.ReponseCodeId!,
            };

            var createDetailTransactionResponse =
                await _detailTransactionServices.InsertDetailTransaction(detailTransactionRequest);
            if (createDetailTransactionResponse.IsSuccess && result)
                return new BaseResultWithData<CreatePaymentResponse>
                {
                    IsSuccess = true,
                    Data = new CreatePaymentResponse
                    {
                        PaymentTransactionId = paymentTransactionModel.PaymentTransactionId
                    },
                    Message = "Create new payment transaction success",
                    StatusCode = StatusCodes.Status201Created
                };

            return new BaseResultWithData<CreatePaymentResponse>
            {
                IsSuccess = false,
                Message = "Adding a new record failed!",
                StatusCode = StatusCodes.Status404NotFound
            };
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw new Exception(e.Message);
        }
    }

    public async Task<BaseResult> UpdatePaymentTransactionAsync(PaymentCompletion paymentCompletion)
    {
        var query = Extensions.GetUpdateQuery("PaymentTransaction", "PaymentTransactionId", "PaymentStatus",
                       "PaymentLastMessage", "PaymentCompletionTime");
        var result = await _db.SaveData(query, paymentCompletion);
        if (result)
            return new BaseResult
            {
                IsSuccess = true,
                Message = "Update payment transaction success",
                StatusCode = StatusCodes.Status200OK
            };
        return new BaseResult
        {
            IsSuccess = false
        };
    }
}