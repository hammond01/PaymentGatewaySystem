using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using PaymentGateway.Domain.Common.ResponseBase;
using PaymentGateway.Domain.Constants;
using PaymentGateway.Domain.Repositories;
using PaymentGateway.Domain.Repositories.VNPaySandBox;
using PaymentGateway.Domain.Response.VNPaySandBox;
using PaymentGateway.Infrastructure.VNPaySandBox.Lib;
using System.Globalization;
using System.Text.Json;
using static PaymentGateway.Domain.Request.VNPaySandBox.VNPaySanBoxRequest;

namespace PaymentGateway.Infrastructure.VNPaySandBox.Services;

public class VnPaySandBoxServices : IVNPaySandBoxServices
{
    private readonly IConfiguration _configuration;
    private readonly IPaymentServices _paymentServices;

    public VnPaySandBoxServices(IConfiguration configuration, IPaymentServices paymentServices)
    {
        _configuration = configuration;
        _paymentServices = paymentServices;
    }

    public async Task<BaseResultWithData<PaymentUrlResponse>> CreatePaymentUrl(HttpContext context, CreateStringUrl urlString)
    {
        var tick = DateTime.Now.Ticks.ToString();
        var vnPay = new VnPayLibrary();
        vnPay.AddRequestData("vnp_Version", _configuration["VNPaySanBox:Version"]);
        vnPay.AddRequestData("vnp_Command", _configuration["VNPaySanBox:Command"]);
        vnPay.AddRequestData("vnp_TmnCode", _configuration["VNPaySanBox:TmnCode"]);
        vnPay.AddRequestData("vnp_Locale", _configuration["VNPaySanBox:Locale"]);
        vnPay.AddRequestData("vnp_CurrCode", _configuration["VNPaySanBox:CurrCode"]);
        vnPay.AddRequestData("vnp_ReturnUrl", _configuration["VNPaySanBox:PaymentBackReturnUrl"]);

        vnPay.AddRequestData("vnp_Amount", (urlString.Amount * 100).ToString(CultureInfo.InvariantCulture));
        vnPay.AddRequestData("vnp_CreateDate", urlString.CreatedDate.ToString("yyyyMMddHHmmss"));

        vnPay.AddRequestData("vnp_IpAddr", Utils.GetIpAddress(context));
        vnPay.AddRequestData("vnp_OrderInfo", "Thanh toán cho đơn hàng:" + urlString.OrderId);
        vnPay.AddRequestData("vnp_OrderType", "other"); //default value: other
        vnPay.AddRequestData("vnp_TxnRef", tick);
        var paymentUrl = vnPay.CreateRequestUrl(_configuration["VNPaySanBox:BaseUrl"],
            _configuration["VNPaySanBox:HashSecret"]);

        //save to database
        var createPayment = new CreatePayment()
        {
            PaymentId = Guid.NewGuid().ToString(),
            PaymentContent = urlString.PaymentContent,
            PaymentCurrency = "VND", //default value: VND
            PaidAmount = urlString.Amount,
            PaymentDate = DateTime.Now,
            ExpireDate = DateTime.Now.AddMinutes(15),
            PaymentLanguage = "VN",//default value: VN
            MerchantId = urlString.MerchantId,
            PaymentStatus = PaymentStatusConstants.Pending,
        };

        var createPaymentAsync = await _paymentServices.CreatePaymentAsync(createPayment);

        var json = JsonDocument.Parse(createPaymentAsync.ToString()!).RootElement;

        if (json.GetProperty("IsSuccess").GetBoolean() == true)
            return new BaseResultWithData<PaymentUrlResponse>
            {
                IsSuccess = true,
                Data = new PaymentUrlResponse
                {
                    PaymentUrl = paymentUrl
                },
                Message = "Create payment success",
            };
        return new BaseResultWithData<PaymentUrlResponse>
        {
            IsSuccess = false,
            Data = new PaymentUrlResponse
            {
                PaymentUrl = string.Empty
            },
            Message = "Create payment fail",
        };
    }

    public async Task<VNPaySandBoxResponse> PaymentExecute(IQueryCollection queryCollection)
    {
        await Task.Delay(500);

        var vnPay = new VnPayLibrary();


        foreach (var (key, value) in queryCollection)
            if (!string.IsNullOrEmpty(value) && key.StartsWith("vnp_"))
                vnPay.AddResponseData(key, value.ToString());

        var vnp_orderId = Convert.ToInt64(vnPay.GetResponseData("vnp_TxnRef"));
        var vnp_TransactionId = Convert.ToInt64(vnPay.GetResponseData("vnp_TransactionNo"));
        var vnp_SecureHash = queryCollection.FirstOrDefault(p => p.Key == "vnp_SecureHash").Value;
        var vnp_ResponseCode = vnPay.GetResponseData("vnp_ResponseCode");
        var vnp_OrderInfo = vnPay.GetResponseData("vnp_OrderInfo");
        var checkSignature = vnPay.ValidateSignature(vnp_SecureHash, _configuration["VNPaySanBox:HashSecret"]);
        if (!checkSignature)
            return new VNPaySandBoxResponse { Success = false };
        var vnPayResponse = new VNPaySandBoxResponse
        {
            Success = true,
            PaymentMethod = vnPay.GetResponseData("vnp_PayMethod"),
            OrderDescription = vnPay.GetResponseData("vnp_OrderInfo"),
            OrderId = vnPay.GetResponseData("vnp_TxnRef"),
            PaymentId = vnPay.GetResponseData("vnp_TransactionNo"),
            TransactionId = vnp_TransactionId.ToString(),
            Token = vnp_SecureHash!,
            VnPayResponseCode = vnp_ResponseCode
        };
        return vnPayResponse;
    }
}