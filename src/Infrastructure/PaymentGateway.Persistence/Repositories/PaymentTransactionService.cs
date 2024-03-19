using Microsoft.AspNetCore.Http;
using PaymentGateway.Domain.Common.ResponseBase;
using PaymentGateway.Domain.Constants;
using PaymentGateway.Domain.Entities;
using PaymentGateway.Domain.Repositories;
using PaymentGateway.Ultils.ConfigDBConnection.Impl;
using PaymentGateway.Ultils.Extension;
using Serilog;

namespace PaymentGateway.Persistence.Repositories;

public class PaymentTransactionService : IPaymentTransactionService
{
    private readonly IDataAccess _db;
    private readonly IDetailTransactionServices _detailTransactionServices;
    private readonly ITransactionCodeService _transactionCodeService;

    public PaymentTransactionService(IDataAccess db, IDetailTransactionServices detailTransactionServices,
        ITransactionCodeService transactionCodeService)
    {
        _db = db;
        _detailTransactionServices = detailTransactionServices;
        _transactionCodeService = transactionCodeService;
    }

    public async Task<BaseResultWithData<string>> CreatePaymentTransactionAsync(
        CreatePaymentTransactionModel paymentTransactionRequest)
    {
        try
        {
            var query = Extensions.GetInsertQuery("PaymentTransaction", "PaymentTransactionId",
                "PaymentContent", "PaymentCurrency", "PaymentDate",
                "ExpireDate", "PaymentLanguage", "MerchantId", "PaidAmount", "PaymentStatus", "PaymentLastMessage",
                "PaymentCompletionTime");
            var paymentTransactionModel = PaymentTransaction.GeneratePaymentTransaction(paymentTransactionRequest);
            var result = await _db.SaveData(query, paymentTransactionModel);
            if (result)
            {
                Log.Information(MessageConstantsWithValue.createSuccess("payment transaction"));
                //create detail transaction
                var paymentStatus =
                    await _transactionCodeService.GetTransactionCodeByTypeAsync(RequestTypeTransactionConstants
                        .COMMON_TRANSACTION);
                var detailTransactionRequest = new CreateDetailTransaction
                {
                    DetailTransactionName = "Create new payment transaction",
                    DetailTransactionIpAddress = paymentTransactionRequest.IpAddress!,
                    DetailTransactionUserId = "User Id example",
                    TransactionId = paymentTransactionModel.PaymentTransactionId,
                    ReponseCodeId = paymentStatus
                        .FirstOrDefault(x => x.ResponseCode!.Trim() == ResponseCodeConstants.AWAITING_PAYMENT)
                        ?.ReponseCodeId!
                };

                var createDetailTransactionResponse =
                    await _detailTransactionServices.InsertDetailTransaction(detailTransactionRequest);

                if (createDetailTransactionResponse.IsSuccess)
                {
                    Log.Information(MessageConstantsWithValue.createSuccess("detail transaction"));

                    return new BaseResultWithData<string>
                    {
                        IsSuccess = true,
                        Data = paymentTransactionModel.PaymentTransactionId,
                        Message = MessageConstantsWithValue.createSuccess("payment transaction and detail transaction"),
                        StatusCode = StatusCodes.Status201Created
                    };
                }

                Log.Error(MessageConstantsWithValue.createFail("detail transaction"));
                return new BaseResultWithData<string>
                {
                    IsSuccess = true,
                    Data = paymentTransactionModel.PaymentTransactionId,
                    Message = MessageConstantsWithValue.createSuccess("payment transaction and detail transaction"),
                    StatusCode = StatusCodes.Status201Created
                };
            }

            Log.Error(MessageConstantsWithValue.createFail("payment transaction"));

            return new BaseResultWithData<string>
            {
                IsSuccess = false,
                Message = MessageConstantsWithValue.createFail("payment transaction"),
                Data = string.Empty,
                StatusCode = StatusCodes.Status404NotFound
            };
        }
        catch
        {
            Log.Error(MessageConstants.InternalServerError);
            throw;
        }
    }

    public async Task<BaseResult> UpdatePaymentTransactionAsync(PaymentCompletion paymentCompletion)
    {
        try
        {
            var query = Extensions.GetUpdateQuery("PaymentTransaction", "PaymentTransactionId", "PaymentStatus",
                "PaymentLastMessage", "PaymentCompletionTime");
            var result = await _db.SaveData(query, paymentCompletion);
            if (result)
            {
                Log.Information(MessageConstantsWithValue.updateSuccess("payment transaction"));
                return new BaseResult
                {
                    IsSuccess = true,
                    Message = "Update payment transaction success",
                    StatusCode = StatusCodes.Status200OK
                };
            }

            Log.Error(MessageConstantsWithValue.updateFail("payment transaction"));
            return new BaseResult
            {
                IsSuccess = false
            };
        }
        catch
        {
            Log.Error(MessageConstants.InternalServerError);
            throw;
        }
    }

    public async Task<BaseResultWithData<CheckTransactionStatus>> CheckTransactionStatus(string transactionId)
    {
        try
        {
            var query = @"SELECT p.PaymentTransactionId,
                               p.PaymentContent,
                               m.MerchantName,
                               p.PaidAmount,
                               p.PaymentStatus,
                               p.PaymentCompletionTime
                        FROM PaymentTransaction p
                        LEFT JOIN Merchant m ON p.MerchantId = m.MerchantId
                        WHERE p.PaymentTransactionId = @transactionId";
            var result = await _db.GetData<CheckTransactionStatus, dynamic>(query, new { transactionId })!;

            var checkTransactionStatusEnumerable = result as List<CheckTransactionStatus> ?? result.ToList();
            if (checkTransactionStatusEnumerable.Any())
            {
                Log.Information(MessageConstantsWithValue.getDataSuccess("transaction status"));
                return new BaseResultWithData<CheckTransactionStatus>
                {
                    IsSuccess = true,
                    Data = checkTransactionStatusEnumerable.FirstOrDefault(),
                    Message = MessageConstantsWithValue.getDataSuccess("transaction status"),
                    StatusCode = StatusCodes.Status200OK
                };
            }
            Log.Error(MessageConstantsWithValue.getDataFail("transaction status"));
            return new BaseResultWithData<CheckTransactionStatus>
            {
                IsSuccess = false,
                Message = MessageConstantsWithValue.getDataFail("transaction status"),
                StatusCode = StatusCodes.Status404NotFound
            };
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}