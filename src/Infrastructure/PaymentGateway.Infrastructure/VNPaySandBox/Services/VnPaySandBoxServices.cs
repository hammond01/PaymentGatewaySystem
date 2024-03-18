using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using PaymentGateway.Domain.Common.ResponseBase;
using PaymentGateway.Domain.Constants;
using PaymentGateway.Domain.Entities;
using PaymentGateway.Domain.Repositories;
using PaymentGateway.Domain.Repositories.VNPaySandBox;
using PaymentGateway.Domain.Request;
using PaymentGateway.Domain.Response.VNPaySandBox;
using PaymentGateway.Infrastructure.VNPaySandBox.Lib;
using System.Globalization;
using static PaymentGateway.Domain.Request.VNPaySandBox.VNPaySanBoxRequest;

namespace PaymentGateway.Infrastructure.VNPaySandBox.Services;

public class VnPaySandBoxServices : IVNPaySandBoxServices
{
    private readonly IConfiguration _configuration;
    private readonly IPaymentTransactionService _paymentTransactionService;
    private readonly ITransactionCodeService _transactionCodeService;
    private readonly IDetailTransactionServices _detailTransactionServices;

    public VnPaySandBoxServices(IConfiguration configuration, IPaymentTransactionService paymentTransactionService,
        ITransactionCodeService transactionCodeService, IDetailTransactionServices detailTransactionServices)
    {
        _configuration = configuration;
        _paymentTransactionService = paymentTransactionService;
        _transactionCodeService = transactionCodeService;
        _detailTransactionServices = detailTransactionServices;
    }

    public async Task<BaseResultWithData<PaymentUrlResponse>> CreatePaymentUrl(HttpContext context,
        CreateStringUrl urlString)
    {
        try
        {
            var tick = DateTime.Now.Ticks.ToString();
            var vnPay = new VnPayLibrary();

            #region AddRequestData

            vnPay.AddRequestData("vnp_Version", _configuration["VNPaySanBox:Version"]);
            vnPay.AddRequestData("vnp_Command", _configuration["VNPaySanBox:Command"]);
            vnPay.AddRequestData("vnp_TmnCode", _configuration["VNPaySanBox:TmnCode"]);
            vnPay.AddRequestData("vnp_Locale", _configuration["VNPaySanBox:Locale"]);
            vnPay.AddRequestData("vnp_CurrCode", _configuration["VNPaySanBox:CurrCode"]);
            vnPay.AddRequestData("vnp_ReturnUrl", _configuration["VNPaySanBox:PaymentBackReturnUrl"]);

            vnPay.AddRequestData("vnp_Amount", (urlString.Amount * 100).ToString(CultureInfo.InvariantCulture));
            vnPay.AddRequestData("vnp_CreateDate", DateTime.Now.ToString("yyyyMMddHHmmss"));

            vnPay.AddRequestData("vnp_IpAddr", Utils.GetIpAddress(context));
            vnPay.AddRequestData("vnp_OrderInfo", "Thanh toán cho đơn hàng: " + urlString.OrderId);
            vnPay.AddRequestData("vnp_OrderType", "other"); //default value: other
            vnPay.AddRequestData("vnp_TxnRef", tick);
            var paymentUrl = vnPay.CreateRequestUrl(_configuration["VNPaySanBox:BaseUrl"],
                _configuration["VNPaySanBox:HashSecret"]);

            #endregion

            //Customer create Payment
            var createPayment = new PaymentTransactionRequest
            {
                PaymentTransactionId = tick,
                PaymentContent = urlString.PaymentContent,
                PaymentCurrency = "VND", //default value: VND
                PaidAmount = urlString.Amount,
                PaymentLanguage = "VN", //default value: VN
                MerchantId = urlString.MerchantId,
                PaymentStatus = PaymentStatusConstants.Pending,
                IpAddress = Utils.GetIpAddress(context)
            };

            var createPaymentAsync = await _paymentTransactionService.CreatePaymentTransactionAsync(createPayment);

            if (createPaymentAsync.IsSuccess)
                return new BaseResultWithData<PaymentUrlResponse>
                {
                    IsSuccess = true,
                    Data = new PaymentUrlResponse
                    {
                        PaymentUrl = paymentUrl
                    },
                    Message = "Create payment success."
                };
            return new BaseResultWithData<PaymentUrlResponse>
            {
                IsSuccess = false,
                Data = new PaymentUrlResponse
                {
                    PaymentUrl = string.Empty
                },
                Message = "Create payment fail."
            };
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw new Exception(e.Message);
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
            var paymentTransactionId = vnPay.GetResponseData("vnp_TxnRef");
            var transactionNo = Convert.ToInt64(vnPay.GetResponseData("vnp_TransactionNo"));
            var vnp_SecureHash = queryCollection.FirstOrDefault(p => p.Key == "vnp_SecureHash").Value;
            var vnp_ResponseCode = vnPay.GetResponseData("vnp_ResponseCode");
            var vnp_OrderInfo = vnPay.GetResponseData("vnp_OrderInfo");
            var vnp_BankCode = vnPay.GetResponseData("vnp_BankCode");
            var completeTime = vnPay.GetResponseData("vnp_PayDate");
            var checkSignature = vnPay.ValidateSignature(vnp_SecureHash, _configuration["VNPaySanBox:HashSecret"]);
            if (!checkSignature)
                return new BaseResultWithData<object> { IsSuccess = false };

            var checkMessage =
                await _transactionCodeService.GetTransactionCodeByCodeAsync(vnp_ResponseCode,
                    RequestTypeTransactionConstants.RETURN_URL);
            var ipAddress = Utils.GetIpAddress(context);
            var detailTransactionRequest = new DetailTransactionRequest
            {
                DetailTransactionName = "Processing payment transactions",
                DetailTransactionIpAddress = ipAddress,
                DetailTransactionUserId = "User Id example line 121 file VNPaySanbox Service",
                TransactionId = paymentTransactionId,
                ReponseCodeId = checkMessage.ReponseCodeId!,
                BankCode = vnp_BankCode
            };

            var createDetailTransactionResponse =
                await _detailTransactionServices.InsertDetailTransaction(detailTransactionRequest);
            var paymentCompletion = new PaymentCompletion
            {
                PaymentStatus = checkMessage.Message!,
                PaymentLastMessage = "143 File VNPay Sandbox service",
                PaymentTransactionId = paymentTransactionId,
                PaymentCompletionTime = DateTime
                    .ParseExact(completeTime, "yyyyMMddHHmmss", CultureInfo.InvariantCulture).ToString("MM/dd/yyyy")
            };
            var updatePaymentTransaction =
                await _paymentTransactionService.UpdatePaymentTransactionAsync(paymentCompletion);
            //update payment transaction by paymentTransactionId

            return new BaseResultWithData<object>
            {
                IsSuccess = vnp_ResponseCode == "00",
                Message = checkMessage.Message,
                Data = new
                {
                    paymentTransactionId,
                    transactionNo,
                    vnp_ResponseCode,
                    vnp_OrderInfo,
                    createDetailTransactionResponse,
                    vnp_BankCode,
                    ipAddress
                },
                StatusCode = vnp_ResponseCode == "00"
                    ? StatusCodes.Status200OK
                    : StatusCodes.Status400BadRequest
            };
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw new Exception(e.Message);
        }
    }
}