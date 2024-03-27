using Microsoft.AspNetCore.Http;
using PaymentGateway.Domain.Common.ResponseBase;
using PaymentGateway.Domain.Constants;
using PaymentGateway.Domain.Entities;
using PaymentGateway.Domain.Exceptions.ErrorMessage;
using PaymentGateway.Domain.Repositories;
using PaymentGateway.Ultils.ConfigDBConnection.Impl;
using Serilog;

namespace PaymentGateway.Persistence.Repositories;

public class PaymentTransactionRepository : IPaymentTransactionService
{
    private readonly IDataAccess _db;
    private readonly IDetailTransactionServices _detailTransactionServices;
    private readonly ITransactionCodeService _transactionCodeService;

    public PaymentTransactionRepository(IDataAccess db, IDetailTransactionServices detailTransactionServices,
        ITransactionCodeService transactionCodeService)
    {
        _db = db;
        _detailTransactionServices = detailTransactionServices;
        _transactionCodeService = transactionCodeService;
    }

    public async Task<BaseResultWithData<long>> CreatePaymentTransactionAsync(
        CreatePaymentTransactionModel paymentTransactionRequest)
    {
        try
        {
            var paymentTransactionModel = PaymentTransaction.GeneratePaymentTransaction(paymentTransactionRequest);
            var result = await _db.InsertData("PaymentTransaction", paymentTransactionModel);
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
                    DetailTransactionUserId = paymentTransactionRequest.UserId!,
                    TransactionId = paymentTransactionModel.PaymentTransactionId,
                    ResponseCodeId = paymentStatus.FirstOrDefault(x => x.ResponseCode!.Trim() == ResponseCodeConstants.AWAITING_PAYMENT)!.ResponseCodeId
                };

                var createDetailTransactionResponse =
                    await _detailTransactionServices.InsertDetailTransaction(detailTransactionRequest);

                if (createDetailTransactionResponse.IsSuccess)
                {
                    Log.Information(MessageConstantsWithValue.createSuccess("detail transaction"));

                    return new BaseResultWithData<long>
                    {
                        IsSuccess = true,
                        Data = paymentTransactionModel.PaymentTransactionId,
                        Message = MessageConstantsWithValue.createSuccess("payment transaction and detail transaction"),
                        StatusCode = StatusCodes.Status201Created
                    };
                }

                Log.Error(MessageConstantsWithValue.createFail("detail transaction", ""));
                return new BaseResultWithData<long>
                {
                    IsSuccess = true,
                    Data = paymentTransactionModel.PaymentTransactionId,
                    Message = MessageConstantsWithValue.createSuccess("payment transaction and detail transaction"),
                    StatusCode = StatusCodes.Status201Created
                };
            }

            Log.Error(MessageConstantsWithValue.createFail("payment transaction", ""));

            return new BaseResultWithData<long>
            {
                IsSuccess = false,
                Message = MessageConstantsWithValue.createFail("payment transaction", ""),
                StatusCode = StatusCodes.Status404NotFound
            };
        }
        catch (Exception e)
        {
            Log.Error(LayerErrorMessage.ERROR_AT_PERSISTENCE(e.Message));
            throw;
        }
    }

    public async Task<BaseResult> UpdatePaymentTransactionAsync(long updatedId, PaymentCompletion paymentCompletion)
    {
        try
        {
            var result = await _db.UpdateData("PaymentTransaction", updatedId, paymentCompletion);
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

            Log.Error(MessageConstantsWithValue.updateFail("payment transaction", ""));
            return new BaseResult
            {
                IsSuccess = false
            };
        }
        catch (Exception e)
        {
            Log.Error(LayerErrorMessage.ERROR_AT_PERSISTENCE(e.Message));
            throw;
        }
    }

    public async Task<BaseResultWithData<CheckTransactionStatus>> CheckTransactionStatus(long transactionNo)
    {
        try
        {
            var query = @"SELECT dp.TransactionNo,
                               pt.PaymentContent,
                               pt.PaymentCompletionTime,
                               m.MerchantName,
                               pt.PaidAmount,
                               pt.PaymentStatus
                        FROM DetailPayment dp
                        LEFT JOIN PaymentTransaction pt ON dp.PaymentTransactionId = pt.PaymentTransactionId
                        LEFT JOIN Merchant m ON pt.MerchantId = m.MerchantId
                        WHERE dp.TransactionNo = @transactionNo";
            var result = await _db.GetData<CheckTransactionStatus, dynamic>(query, new { transactionNo });

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
            Log.Error(MessageConstantsWithValue.getDataFail("transaction status", ""));
            return new BaseResultWithData<CheckTransactionStatus>
            {
                IsSuccess = false,
                Message = $"No transaction exists with transaction code: {transactionNo}",
                StatusCode = StatusCodes.Status404NotFound
            };
        }
        catch (Exception e)
        {
            Log.Error(LayerErrorMessage.ERROR_AT_PERSISTENCE(e.Message));
            throw;
        }
    }
}