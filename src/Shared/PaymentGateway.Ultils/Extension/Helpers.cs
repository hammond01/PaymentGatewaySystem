using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using PaymentGateway.Domain.Entities.ThirdParty;
using PaymentGateway.Domain.Request;
using System.Net;
using System.Security.Cryptography;
using System.Text;

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

        var serviceCode = _configuration["JsonStringVNPay:serviceCode"]!; //default
        var countryCode = _configuration["JsonStringVNPay:countryCode"]!; //default
        var payType = _configuration["JsonStringVNPay:payType"]!; //default
        var masterMerCode = _configuration["JsonStringVNPay:masterMerCode"]!; //default
        var ccy = _configuration["JsonStringVNPay:ccy"]!; //default
        var expDate = _configuration["JsonStringVNPay:expDate"]!; //default

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
        return Task.Run(() =>
        {
            using var md5 = MD5.Create();
            var inputBytes = Encoding.ASCII.GetBytes(input);
            var hashBytes = md5.ComputeHash(inputBytes);
            var sb = new StringBuilder();
            foreach (var t in hashBytes)
                sb.Append(t.ToString("X2"));
            return sb.ToString();
        });
    }
    public string GetClientIpAddress()
    {
        var ipAddress = _httpContextAccessor.HttpContext!.Connection.RemoteIpAddress;

        // Kiểm tra nếu là IPv6 loopback (::1), trả về IPv4 loopback (127.0.0.1)
        if (ipAddress!.IsIPv6LinkLocal || ipAddress.IsIPv6SiteLocal || ipAddress.IsIPv6Multicast)
        {
            return IPAddress.Loopback.ToString(); // Trả về IPv4 loopback (127.0.0.1)
        }

        // Chuyển đổi IPv6 thành IPv4 nếu có thể
        if (ipAddress.AddressFamily == System.Net.Sockets.AddressFamily.InterNetworkV6)
        {
            return ipAddress.MapToIPv4().ToString();
        }

        // Nếu không phải IPv6 hoặc không thể chuyển đổi, trả về địa chỉ IP ban đầu
        return ipAddress.ToString();
    }
}