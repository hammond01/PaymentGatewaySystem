using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using PaymentGateway.Domain.Common.ResponseBase;
using PaymentGateway.Domain.Constants;
using PaymentGateway.Domain.Entities;
using PaymentGateway.Domain.Entities.ThirdParty.VNPayEntities;
using PaymentGateway.Domain.Repositories;
using PaymentGateway.Domain.Repositories.VNPaySandBox;
using PaymentGateway.Infrastructure.VNPaySandBox.Lib;
using Serilog;
using System.Globalization;

namespace PaymentGateway.Infrastructure.VNPaySandBox.Services;

public class VnPaySandBoxServices : IVNPaySandBoxServices
{
    private readonly IConfiguration _configuration;
    private readonly IPaymentTransactionService _paymentTransactionService;
    private readonly ITransactionCodeService _transactionCodeService;
    private readonly IDetailTransactionServices _detailTransactionServices;
    private readonly HttpClient _client;

    public VnPaySandBoxServices(IConfiguration configuration, IPaymentTransactionService paymentTransactionService,
        ITransactionCodeService transactionCodeService, IDetailTransactionServices detailTransactionServices, HttpClient client)
    {
        _configuration = configuration;
        _paymentTransactionService = paymentTransactionService;
        _transactionCodeService = transactionCodeService;
        _detailTransactionServices = detailTransactionServices;
        _client = client;
    }

    public async Task<BaseResultWithData<string>> CreatePaymentUrl(HttpContext context,
        CreateStringUrlRequest urlString)
    {
        try
        {
            var tick = DateTime.Now.Ticks.ToString();
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
                OrderType = "other", //default value: other
                TxnRef = tick,
                BaseUrl = _configuration["VNPaySanBox:BaseUrl"]!,
                HashSecret = _configuration["VNPaySanBox:HashSecret"]!
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
            vnPay.AddRequestData("vnp_TxnRef", createPaymentUrlModel.TxnRef);
            var paymentUrl = vnPay.CreateRequestUrl(_configuration["VNPaySanBox:BaseUrl"],
                _configuration["VNPaySanBox:HashSecret"]);

            #endregion

            //Customer create Payment
            var createPayment = new CreatePaymentTransactionModel
            {
                PaymentTransactionId = tick,
                PaymentContent = urlString.PaymentContent,
                PaymentCurrency = createPaymentUrlModel.CurrCode,
                PaidAmount = urlString.Amount,
                PaymentLanguage = createPaymentUrlModel.Locale.ToUpper(),
                MerchantId = urlString.MerchantId,
                PaymentStatus = PaymentStatusConstants.Pending,
                IpAddress = createPaymentUrlModel.IpAddr
            };
            //add paymnet transaction to database
            var createPaymentAsync = await _paymentTransactionService.CreatePaymentTransactionAsync(createPayment);

            if (createPaymentAsync.IsSuccess)
                return new BaseResultWithData<string>
                {
                    IsSuccess = true,
                    Data = paymentUrl,
                    Message = MessageConstantsWithValue.createSuccess("payment"),
                    StatusCode = StatusCodes.Status201Created
                };
            return new BaseResultWithData<string>
            {
                IsSuccess = false,
                Data = string.Empty,
                Message = MessageConstantsWithValue.createFail("payment"),
                StatusCode = StatusCodes.Status400BadRequest
            };
        }
        catch
        {
            Log.Error(MessageConstants.InternalServerError);
            throw;
        }
    }

    //When the customer has completed the payment, the bank will return the result to the website.
    public async Task<BaseResultWithData<object>> PaymentExecute(HttpContext context, IQueryCollection queryCollection)
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
                TransactionNo = vnPay.GetResponseData("vnp_TransactionNo"),
                ResponseCode = vnPay.GetResponseData("vnp_ResponseCode"),
                TransactionStatus = vnPay.GetResponseData("vnp_TransactionStatus"),
                TxnRef = vnPay.GetResponseData("vnp_TxnRef"),
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
                    DetailTransactionUserId = "User Id example line 150 file VNPaySanbox Service",
                    TransactionId = createPaymentUrlResponse.TxnRef,
                    ReponseCodeId = checkMessage.ReponseCodeId!,
                    BankCode = createPaymentUrlResponse.BankCode
                };

                var createDetailTransactionResponse =
                    await _detailTransactionServices.InsertDetailTransaction(createDetailTransactionModel);
                var paymentCompletion = new PaymentCompletion
                {
                    PaymentStatus = checkMessage.Message!,
                    PaymentLastMessage = "Line 168 File VNPay Sandbox service",
                    PaymentTransactionId = createPaymentUrlResponse.TxnRef,
                    PaymentCompletionTime = DateTime
                            .ParseExact(createPaymentUrlResponse.PayDate, "yyyyMMddHHmmss",
                                CultureInfo.InvariantCulture)
                            .ToString("MM/dd/yyyy HH:mm:ss")
                };
                await _paymentTransactionService.UpdatePaymentTransactionAsync(paymentCompletion);

                return new BaseResultWithData<object>
                {
                    IsSuccess = createPaymentUrlResponse.ResponseCode == "00",
                    Message = checkMessage.Message,
                    Data = new
                    {
                        createPaymentUrlResponse,
                        ipAddress
                    },
                    StatusCode = createPaymentUrlResponse.ResponseCode == "00"
                    ? StatusCodes.Status200OK
                    : StatusCodes.Status400BadRequest
                };
            }

            Log.Error("Invalid signature");
            return new BaseResultWithData<object>
            {
                IsSuccess = false,
                Message = checkMessage.Message,
                StatusCode = StatusCodes.Status400BadRequest,
                Data = string.Empty
            };
        }
        catch (Exception e)
        {
            Log.Error(MessageConstants.InternalServerError);
            throw new Exception(e.Message);
        }
    }

    public async Task<BaseResultWithData<object>> Refund(HttpContext context, RefundRequestClient refundRequest)
    {
        try
        {
            var tick = DateTime.Now.Ticks.ToString();
            var refundRequestModel = new RefundRequest
            {
                RequestId = tick,
                Version = _configuration["VNPaySanBox:Version"]!,
                Command = "refund",
                TmnCode = _configuration["VNPaySanBox:TmnCode"]!,
                TransactionType = refundRequest.TransactionType,
                TxnRef = refundRequest.TxnRef,
                Amount = Convert.ToInt64(refundRequest.Amount) * 100,
                OrderInfo = "Hoan tien giao dich:" + refundRequest.TxnRef,
                TransactionNo = refundRequest.TransactionNo,
                CreateBy = refundRequest.CreateBy,
                CreateDate = DateTime.Now.ToString("yyyyMMddHHmmss"),
                IpAddr = Utils.GetIpAddress(context),
                SecureHash = string.Empty,
                TransactionDate = refundRequest.TransactionDate
            };
            var vnp_HashSecret = _configuration["VNPaySanBox:HashSecret"]!;
            var signData = refundRequestModel.RequestId + "|" + refundRequestModel.Version + "|" + refundRequestModel.Command + "|" + refundRequestModel.TmnCode + "|" + refundRequestModel.TransactionType + "|" + refundRequestModel.TxnRef + "|" + refundRequestModel.Amount + "|" + refundRequestModel.TransactionNo + "|" + refundRequestModel.TransactionDate + "|" + refundRequestModel.CreateBy + "|" + refundRequestModel.CreateDate + "|" + refundRequestModel.IpAddr + "|" + refundRequestModel.OrderInfo;

            //var signData = Helpers.signDataRefund(refundRequestModel);
            var vnp_SecureHash = Utils.HmacSHA512(vnp_HashSecret, signData);

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
                vnp_SecureHash
            };
            //var jsonData = JsonConvert.SerializeObject(rfData);
            //var content = new StringContent(jsonData, Encoding.UTF8, "application/json");
            //var response = await _client.PostAsync(_configuration["VNPaySanBox:PaymentRefundUrl"], content);
            return new BaseResultWithData<object>
            {
                Data = new
                {
                    rfData,

                },
                IsSuccess = true,

            };
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}