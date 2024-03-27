using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using PaymentGateway.Domain.Entities.ThirdParty;
using PaymentGateway.Domain.Entities.ThirdParty.VNPayEntities;
using PaymentGateway.Domain.Request;

namespace PaymentGateway.Ultils.Extension;

public class Helpers
{
    public readonly IConfiguration _configuration;
    public readonly IHttpContextAccessor _httpContextAccessor;

    public Helpers(IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
    {
        _configuration = configuration;
        _httpContextAccessor = httpContextAccessor;
    }

    public Task<string> CalculateMD5GenQR(string productId, string txnId, string amount, string tipAndFee)
    {
        var appId = _configuration["InformationAPI:GenQR:appId"]!;
        var secretKey = _configuration["InformationAPI:GenQR:secretKey"]!;

        var merchantCode = _configuration["InformationQR:merchantCode"]!;
        var merchantType = _configuration["InformationQR:merchantType"]!;
        var terminalId = _configuration["InformationQR:terminalId"]!;
        var merchantName = _configuration["InformationQR:merchantName"]!;

        var serviceCode = _configuration["JsonStringVNPay:serviceCode"]!;//default
        var countryCode = _configuration["JsonStringVNPay:countryCode"]!;//default
        var payType = _configuration["JsonStringVNPay:payType"]!;//default
        var masterMerCode = _configuration["JsonStringVNPay:masterMerCode"]!;//default
        var ccy = _configuration["JsonStringVNPay:ccy"]!;//default
        var expDate = _configuration["JsonStringVNPay:expDate"]!;//default

        var data =
            $"{appId}|{merchantName}|{serviceCode}|{countryCode}|{masterMerCode}|{merchantType}|{merchantCode}|{terminalId}|{payType}|{productId}|{txnId}|{amount}|{tipAndFee}|{ccy}|{expDate}|{secretKey}";
        return CaculateGenQR(data);
    }

    public CreateQR CreateQRRequestToCreateQR(CreateQrRequest createQRRequest)
    {
        var checkSumValue = CalculateMD5GenQR(createQRRequest.productId, createQRRequest.txnId,
                                              createQRRequest.amount, createQRRequest.tipAndFee);
        var data = new CreateQR
        {
            appId = _configuration["InformationAPI:GenQR:appId"]!,
            serviceCode = _configuration["JsonStringVNPay:serviceCode"]!,
            countryCode = _configuration["JsonStringVNPay:countryCode"]!,
            payType = _configuration["JsonStringVNPay:payType"]!,
            masterMerCode = _configuration["JsonStringVNPay:masterMerCode"]!,
            ccy = _configuration["JsonStringVNPay:ccy"]!,
            expDate = _configuration["JsonStringVNPay:expDate"]!,
            merchantType = _configuration["InformationQR:merchantType"]!,
            merchantCode = _configuration["InformationQR:merchantCode"]!,
            terminalId = _configuration["InformationQR:terminalId"]!,
            merchantName = _configuration["InformationQR:merchantName"]!,
            checksum = checkSumValue.ToString()!,
            productId = createQRRequest.productId,
            txnId = createQRRequest.txnId,
            amount = createQRRequest.amount,
            tipAndFee = createQRRequest.tipAndFee!,
            desc = createQRRequest.desc,
            billNumber = createQRRequest.billNumber,
            consumerId = "",
            purpose = ""
        };
        return data;
    }

    private Task<string> CaculateGenQR(string input)
    {
        return Task.Run(() => {
            using var md5 = MD5.Create();
            var inputBytes = Encoding.ASCII.GetBytes(input);
            var hashBytes = md5.ComputeHash(inputBytes);
            var sb = new StringBuilder();
            foreach (var t in hashBytes)
                sb.Append(t.ToString("X2"));
            return sb.ToString();
        });
    }

    public static string signDataRefund(RefundRequest refundRequest) => @$"{refundRequest.RequestId} | {refundRequest.Version} | {refundRequest.Command} | {refundRequest.TmnCode} | {refundRequest.TransactionType} | {refundRequest.TxnRef} | {refundRequest.Amount} | {refundRequest.TransactionNo} | {refundRequest.TransactionDate} | {refundRequest.CreateBy} | {refundRequest.CreateDate} | {refundRequest.IpAddr} | {refundRequest.OrderInfo}";
}