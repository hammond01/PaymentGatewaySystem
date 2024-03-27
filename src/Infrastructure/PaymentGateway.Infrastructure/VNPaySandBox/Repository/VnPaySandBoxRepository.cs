﻿using System.Globalization;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using PaymentGateway.Domain.Common.ResponseBase;
using PaymentGateway.Domain.Constants;
using PaymentGateway.Domain.Entities;
using PaymentGateway.Domain.Entities.ThirdParty.VNPayEntities;
using PaymentGateway.Domain.Exceptions.ErrorMessage;
using PaymentGateway.Domain.Repositories;
using PaymentGateway.Domain.Repositories.VNPaySandBox;
using PaymentGateway.Domain.Request;
using PaymentGateway.Infrastructure.VNPaySandBox.Lib;
using PaymentGateway.Ultils.CommonServices;
using PaymentGateway.Ultils.ConfigDBConnection.Impl;
using PaymentGateway.Ultils.Extension;
using Serilog;

namespace PaymentGateway.Infrastructure.VNPaySandBox.Repository;

public class VnPaySandBoxRepository : IVnPaySandBoxServices
{
    private readonly IConfiguration _configuration;
    private readonly IDataAccess _db;
    private readonly IDetailPaymentService _detailPaymentService;
    private readonly IDetailTransactionServices _detailTransactionServices;
    private readonly IPaymentTransactionService _paymentTransactionService;
    private readonly ITransactionCodeService _transactionCodeService;
    private readonly UserIdService _userIdService;

    public VnPaySandBoxRepository(IConfiguration configuration, IPaymentTransactionService paymentTransactionService,
        ITransactionCodeService transactionCodeService, IDetailTransactionServices detailTransactionServices,
        IDetailPaymentService detailPaymentService, IDataAccess db, UserIdService userIdService)
    {
        _configuration = configuration;
        _paymentTransactionService = paymentTransactionService;
        _transactionCodeService = transactionCodeService;
        _detailTransactionServices = detailTransactionServices;
        _detailPaymentService = detailPaymentService;
        _db = db;
        _userIdService = userIdService;
    }
    public async Task<BaseResultWithData<string>> CreatePaymentUrl(HttpContext context,
        CreatePaymentSandboxRequest urlString)
    {
        try
        {
            _userIdService.UserId = urlString.UserId!;
            var tick = Extension.GenerateUniqueId();
            var vnPay = new VnPayLibrary();
            var createPaymentUrlModel = new CreatePaymentURLRequestTransfer
            {
                Version = _configuration["VNPaySanBox:Version"]!,
                Command = _configuration["VNPaySanBox:Command"]!,
                TmnCode = _configuration["VNPaySanBox:TmnCode"]!,
                Locale = _configuration["VNPaySanBox:Locale"]!,
                CurrCode = _configuration["VNPaySanBox:CurrCode"]!,
                ReturnUrl = _configuration["VNPaySanBox:PaymentBackReturnUrl"]!,
                Amount = (urlString.Amount * 100).ToString(CultureInfo.InvariantCulture),
                CreateDate = DateTime.Now.ToString("yyyyMMddHHmmss"),
                IpAddr = Utils.GetIpAddress(context),
                OrderInfo = "Thanh toán cho đơn hàng: " + urlString.OrderId,
                OrderType = "other",//default value: other
                TxnRef = tick,
                BaseUrl = _configuration["VNPaySanBox:BaseUrl"]!,
                HashSecret = _configuration["VNPaySanBox:HashSecret"]!
            };

            var checkMessage =
                await _transactionCodeService.GetTransactionCodeByCodeAsync(ResponseCodeConstants.AWAITING_PAYMENT,
                                                                            RequestTypeTransactionConstants.COMMON_TRANSACTION);
            //Customer create Payment
            var createPayment = new CreatePaymentTransactionModel
            {
                PaymentTransactionId = tick,
                PaymentContent = urlString.PaymentContent,
                PaymentCurrency = createPaymentUrlModel.CurrCode,
                PaidAmount = urlString.Amount,
                PaymentLanguage = createPaymentUrlModel.Locale.ToUpper(),
                MerchantId = urlString.MerchantId,
                PaymentStatus = checkMessage.Message,
                IpAddress = createPaymentUrlModel.IpAddr,
                ResponseCodeId = checkMessage.ResponseCodeId,
                Channel = urlString.Channel,
                ClientName = urlString.ClientName,
                PaymentLastMessage = urlString.LastMessage,
                UserId = urlString.UserId!
            };
            //add paymnet transaction to database
            var createPaymentAsync = await _paymentTransactionService.CreatePaymentTransactionAsync(createPayment);
            if (!createPaymentAsync.IsSuccess)
                return new BaseResultWithData<string>
                {
                    IsSuccess = false,
                    Data = string.Empty,
                    Message = MessageConstantsWithValue.createFail("payment", ""),
                    StatusCode = StatusCodes.Status400BadRequest
                };
            #region AddRequestData

            vnPay.AddRequestData("vnp_Version", createPaymentUrlModel.Version);
            vnPay.AddRequestData("vnp_Command", createPaymentUrlModel.Command);
            vnPay.AddRequestData("vnp_TmnCode", createPaymentUrlModel.TmnCode);
            vnPay.AddRequestData("vnp_Locale", createPaymentUrlModel.Locale);
            vnPay.AddRequestData("vnp_CurrCode", createPaymentUrlModel.CurrCode);
            vnPay.AddRequestData("vnp_ReturnUrl", createPaymentUrlModel.ReturnUrl);
            vnPay.AddRequestData("vnp_Amount", createPaymentUrlModel.Amount);
            vnPay.AddRequestData("vnp_CreateDate", createPaymentUrlModel.CreateDate);
            vnPay.AddRequestData("vnp_IpAddr", createPaymentUrlModel.IpAddr);
            vnPay.AddRequestData("vnp_OrderInfo", createPaymentUrlModel.OrderInfo);
            vnPay.AddRequestData("vnp_OrderType", createPaymentUrlModel.OrderType);
            vnPay.AddRequestData("vnp_TxnRef", createPaymentUrlModel.TxnRef.ToString());
            var paymentUrl = vnPay.CreateRequestUrl(_configuration["VNPaySanBox:BaseUrl"],
                                                    _configuration["VNPaySanBox:HashSecret"]);

            #endregion
            return new BaseResultWithData<string>
            {
                IsSuccess = true,
                Data = paymentUrl,
                Message = MessageConstantsWithValue.createSuccess("payment"),
                StatusCode = StatusCodes.Status201Created
            };
        }
        catch (Exception e)
        {
            Log.Error(LayerErrorMessage.ERROR_AT_INFRASTRUCTURE(e.Message));
            throw;
        }
    }

    //When the customer has completed the payment, the bank will return the result to the website.
    public async Task<BaseResult> PaymentExecute(HttpContext context, IQueryCollection queryCollection)
    {
        try
        {
            var vnPay = new VnPayLibrary();
            foreach (var (key, value) in queryCollection)
                if (!string.IsNullOrEmpty(value) && key.StartsWith("vnp_"))
                    vnPay.AddResponseData(key, value.ToString());
            var createPaymentUrlResponse = new CreateStringUrlResponse
            {
                TmnCode = vnPay.GetResponseData("vnp_TmnCode"),
                Amount = Convert.ToInt64(vnPay.GetResponseData("vnp_Amount")) / 100,
                BankCode = vnPay.GetResponseData("vnp_BankCode"),
                BankTranNo = vnPay.GetResponseData("vnp_BankTranNo"),
                CardType = vnPay.GetResponseData("vnp_CardType"),
                OrderInfo = vnPay.GetResponseData("vnp_OrderInfo"),
                PayDate = vnPay.GetResponseData("vnp_PayDate"),
                TransactionNo = Convert.ToInt64(vnPay.GetResponseData("vnp_TransactionNo")),
                ResponseCode = vnPay.GetResponseData("vnp_ResponseCode"),
                TransactionStatus = vnPay.GetResponseData("vnp_TransactionStatus"),
                TxnRef = Convert.ToInt64(vnPay.GetResponseData("vnp_TxnRef")),
                SecureHash = queryCollection.FirstOrDefault(p => p.Key == "vnp_SecureHash").Value!,
                SecureHashType = vnPay.GetResponseData("vnp_SecureHashType")
            };
            var ipAddress = Utils.GetIpAddress(context);
            var checkMessage =
                await _transactionCodeService.GetTransactionCodeByCodeAsync(createPaymentUrlResponse.ResponseCode,
                                                                            RequestTypeTransactionConstants.RETURN_URL);
            var checkSignature = vnPay.ValidateSignature(createPaymentUrlResponse.SecureHash,
                                                         _configuration["VNPaySanBox:HashSecret"]);
            if (checkSignature)
            {
                Log.Information("Valid signature");
                Log.Information("Transaction message: " + checkMessage.Message);
                var createDetailTransactionModel = new CreateDetailTransaction
                {
                    DetailTransactionName = "Processing payment transactions",
                    DetailTransactionIpAddress = ipAddress,
                    DetailTransactionUserId = _userIdService.UserId,
                    TransactionId = createPaymentUrlResponse.TxnRef,
                    ResponseCodeId = checkMessage.ResponseCodeId,
                    BankCode = createPaymentUrlResponse.BankCode
                };
                await _detailTransactionServices.InsertDetailTransaction(createDetailTransactionModel);
                var paymentTransactionId = createPaymentUrlResponse.TxnRef;
                var paymentCompletion = new PaymentCompletion
                {
                    PaymentStatus = checkMessage.Message!,
                    PaymentLastMessage = "None",
                    PaymentCompletionTime = DateTime
                        .ParseExact(createPaymentUrlResponse.PayDate, "yyyyMMddHHmmss",
                                    CultureInfo.InvariantCulture)
                        .ToString("MM/dd/yyyy HH:mm:ss"),
                    ResponseCodeId = checkMessage.ResponseCodeId
                };
                await _paymentTransactionService.UpdatePaymentTransactionAsync(paymentTransactionId, paymentCompletion);
                if (await CheckTransactionNo(createPaymentUrlResponse.TransactionNo))
                    return new BaseResultWithData<object>
                    {
                        Data = new
                        {
                            createPaymentUrlResponse.TransactionNo,
                            createPaymentUrlResponse.TransactionStatus,
                            PaymentCompletion = DateTime
                                .ParseExact(createPaymentUrlResponse.PayDate, "yyyyMMddHHmmss",
                                            CultureInfo.InvariantCulture)
                                .ToString("MM/dd/yyyy HH:mm:ss")
                        },
                        IsSuccess = createPaymentUrlResponse.ResponseCode == "00",
                        Message = checkMessage.Message,
                        StatusCode = createPaymentUrlResponse.ResponseCode == "00"
                            ? StatusCodes.Status200OK
                            : StatusCodes.Status400BadRequest
                    };
                var createDetailPaymentSandbox = new DetailPaymentVNPSandBox
                {
                    DetailPaymentId = Extension.GenerateUniqueId(),
                    PaymentTransactionId = createPaymentUrlResponse.TxnRef,
                    TmnCode = createPaymentUrlResponse.TmnCode,
                    Amount = createPaymentUrlResponse.Amount,
                    BankCode = createPaymentUrlResponse.BankCode,
                    BankTranNo = createPaymentUrlResponse.BankTranNo,
                    CardType = createPaymentUrlResponse.CardType,
                    PayDate = createPaymentUrlResponse.PayDate,
                    OrderInfo = createPaymentUrlResponse.OrderInfo,
                    TransactionNo = createPaymentUrlResponse.TransactionNo,
                    ResponseCode = createPaymentUrlResponse.ResponseCode,
                    TransactionStatus = createPaymentUrlResponse.TransactionStatus,
                    TxnRef = createPaymentUrlResponse.TxnRef,
                    SecureHashType = createPaymentUrlResponse.SecureHashType,
                    SecureHash = createPaymentUrlResponse.SecureHash,
                    JsonData = JsonConvert.SerializeObject(createPaymentUrlResponse),
                    ResponseCodeId = checkMessage.ResponseCodeId
                };
                await _detailPaymentService.CreateDataToDetailPaymentAsync(createDetailPaymentSandbox);

                return new BaseResultWithData<object>
                {
                    Data = new
                    {
                        createPaymentUrlResponse.TransactionNo,
                        createPaymentUrlResponse.TransactionStatus,
                        PaymentCompletion = DateTime
                            .ParseExact(createPaymentUrlResponse.PayDate, "yyyyMMddHHmmss",
                                        CultureInfo.InvariantCulture)
                            .ToString("MM/dd/yyyy HH:mm:ss")
                    },
                    IsSuccess = createPaymentUrlResponse.ResponseCode == "00",
                    Message = checkMessage.Message,
                    StatusCode = createPaymentUrlResponse.ResponseCode == "00"
                        ? StatusCodes.Status200OK
                        : StatusCodes.Status400BadRequest
                };
            }

            Log.Error("Invalid signature");
            return new BaseResult
            {
                IsSuccess = false,
                Message = checkMessage.Message,
                StatusCode = StatusCodes.Status400BadRequest
            };
        }
        catch (Exception e)
        {
            Log.Error(LayerErrorMessage.ERROR_AT_INFRASTRUCTURE(e.Message));
            throw;
        }
    }

    public async Task<BaseResultWithData<object>> Refund(HttpContext context, RefundRequestClient refundRequest)
    {
        try
        {
            if (!await CheckTransactionNo(refundRequest.TransactionNo))
                return new BaseResultWithData<object>
                {
                    Data = null,
                    IsSuccess = false,
                    Message = $"No transaction exists with transaction code: {refundRequest.TransactionNo}",
                    StatusCode = StatusCodes.Status404NotFound
                };

            var refunRequestInfo =
                await _detailPaymentService.GetTransactionInfoForRefundRequest(refundRequest.TransactionNo);
            var tick = DateTime.Now.Ticks.ToString();
            var amoundInTransaction = await GetMoneyByTransactionNo(refundRequest.TransactionNo);
            var refundRequestModel = new RefundRequest
            {
                RequestId = tick,
                Version = _configuration["VNPaySanBox:Version"]!,
                Command = "refund",
                TmnCode = _configuration["VNPaySanBox:TmnCode"]!,
                TransactionType = _configuration["VNPaySanBox:Refund:TransactionType"]!,
                TxnRef = refunRequestInfo.Data!.TxnRef,
                Amount = amoundInTransaction * 100,
                OrderInfo = "Hoan tien giao dich:" + refunRequestInfo.Data!.TxnRef,
                TransactionNo = refundRequest.TransactionNo,
                CreateBy = refundRequest.CreateBy,
                CreateDate = DateTime.Now.ToString("yyyyMMddHHmmss"),
                IpAddr = Utils.GetIpAddress(context),
                SecureHash = string.Empty,
                TransactionDate = refunRequestInfo.Data!.PayDate
            };
            var hashSecret = _configuration["VNPaySanBox:HashSecret"]!;
            var signData = refundRequestModel.RequestId + "|" + refundRequestModel.Version + "|" +
                           refundRequestModel.Command + "|" + refundRequestModel.TmnCode + "|" +
                           refundRequestModel.TransactionType + "|" + refundRequestModel.TxnRef + "|" +
                           refundRequestModel.Amount + "|" + refundRequestModel.TransactionNo + "|" +
                           refundRequestModel.TransactionDate + "|" + refundRequestModel.CreateBy + "|" +
                           refundRequestModel.CreateDate + "|" + refundRequestModel.IpAddr + "|" +
                           refundRequestModel.OrderInfo;

            //var signData = Helpers.signDataRefund(refundRequestModel);
            var secureHash = Utils.HmacSHA512(hashSecret, signData);

            var rfData = new
            {
                vnp_RequestId = refundRequestModel.RequestId,
                vnp_Version = refundRequestModel.Version,
                vnp_Command = refundRequestModel.Command,
                vnp_TmnCode = refundRequestModel.TmnCode,
                vnp_TransactionType = refundRequestModel.TransactionType,
                vnp_TxnRef = refundRequestModel.TxnRef,
                vnp_Amount = refundRequestModel.Amount,
                vnp_OrderInfo = refundRequestModel.OrderInfo,
                vnp_TransactionNo = refundRequestModel.TransactionNo,
                vnp_TransactionDate = refundRequestModel.TransactionDate,
                vnp_CreateBy = refundRequestModel.CreateBy,
                vnp_CreateDate = refundRequestModel.CreateDate,
                vnp_IpAddr = refundRequestModel.IpAddr,
                vnp_SecureHash = secureHash
            };
            using var client = new HttpClient();
            var content = new StringContent(JsonConvert.SerializeObject(rfData), Encoding.UTF8, "application/json");

            var request = await client.PostAsync(_configuration["VNPaySanBox:PaymentRefundUrl"], content);

            if (request.IsSuccessStatusCode)
            {
                var jsonString = await request.Content.ReadAsStringAsync();
                jsonString = jsonString.Replace("vnp_", "");
                var json = JsonDocument.Parse(jsonString).RootElement;
                var data = json.GetObject<RefundResponse>();
                var checkMessage =
                    await _transactionCodeService.GetTransactionCodeByCodeAsync(data.ResponseCode!,
                                                                                RequestTypeTransactionConstants.REFUND);
                return new BaseResultWithData<object>
                {
                    Data = checkMessage.Message,
                    IsSuccess = true
                };
            }
            return new BaseResultWithData<object>
            {
                Data = null,
                IsSuccess = false
            };
        }
        catch (Exception e)
        {
            Log.Error(LayerErrorMessage.ERROR_AT_INFRASTRUCTURE(e.Message));
            throw;
        }
    }

    public Task<BaseResultWithData<object>> GetTransactionDetail(HttpContext context, string transactionId) => throw new NotImplementedException();

    private async Task<bool> CheckTransactionNo(long transactionNo)
    {
        // Define the SQL query to check for the DetailPayment
        var query = @"SELECT
                    COUNT(1)
                  FROM
                    DetailPayment
                  WHERE
                    TransactionNo = @TransactionNo";
        // Execute the query and retrieve the data
        var data = await _db.GetData<int, dynamic>(query, new { TransactionNo = transactionNo });

        // Return whether the merchant exists or not
        return data.FirstOrDefault() > 0;
    }
    private async Task<long> GetMoneyByTransactionNo(long transactionNo)
    {
        var query = @"SELECT
                    Amount
                  FROM
                    DetailPayment
                  WHERE
                    TransactionNo = @TransactionNo";
        var data = await _db.GetData<long, dynamic>(query, new { TransactionNo = transactionNo });
        return data.FirstOrDefault();
    }
}