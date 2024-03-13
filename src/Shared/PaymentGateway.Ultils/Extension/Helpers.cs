using Microsoft.Extensions.Configuration;
using PaymentGateway.Domain.Entities.ThirdParty;
using PaymentGateway.Domain.Request;
using System.Security.Cryptography;
using System.Text;

namespace PaymentGateway.Ultils.Extension;
public class Helpers
{
    public readonly IConfiguration _configuration;
    public Helpers(IConfiguration configuration)
    {
        _configuration = configuration;
    }
    public Task<string> CalculateMD5(string merchantName, string merchantCode,
        string terminalId, string productId,
        string txnId, string amount, string tipAndFee)
    {
        string appId = _configuration["JsonStringVNPay:appId"]!;
        string secretKey = _configuration["JsonStringVNPay:secretKey"]!;
        string serviceCode = _configuration["JsonStringVNPay:serviceCode"]!;
        string countryCode = _configuration["JsonStringVNPay:countryCode"]!;
        string payType = _configuration["JsonStringVNPay:payType"]!;
        string masterMerCode = _configuration["JsonStringVNPay:masterMerCode"]!;
        string ccy = _configuration["JsonStringVNPay:ccy"]!;
        string expDate = _configuration["JsonStringVNPay:expDate"]!;
        string merchantType = _configuration["JsonStringVNPay:merchantType"]!;

        string data = $"{appId}|{merchantName}|{serviceCode}|{countryCode}|{masterMerCode}|{merchantType}|{merchantCode}|{terminalId}|{payType}|{productId}|{txnId}|{amount}|{tipAndFee}|{ccy}|{expDate}|{secretKey}";
        return Caculate(data);
    }
    public CreateQR CreateQRRequestToCreateQR(CreateQRRequest createQRRequest)
    {
        var data = new CreateQR()
        {
            appId = _configuration["JsonStringVNPay:appId"]!,
            merchantName = createQRRequest.merchantName,
            serviceCode = _configuration["JsonStringVNPay:serviceCode"]!,
            countryCode = _configuration["JsonStringVNPay:countryCode"]!,
            masterMerCode = _configuration["JsonStringVNPay:masterMerCode"]!,
            merchantType = _configuration["JsonStringVNPay:merchantType"]!,
            merchantCode = _configuration["JsonStringVNPay:merchantCode"]!,
            terminalId = createQRRequest.terminalId,
            payType = _configuration["JsonStringVNPay:payType"]!,
            productId = createQRRequest.productId,
            txnId = createQRRequest.txnId,
            amount = createQRRequest.amount,
            tipAndFee = createQRRequest.tipAndFee!,
            ccy = _configuration["JsonStringVNPay:ccy"]!,
            expDate = _configuration["JsonStringVNPay:expDate"]!,
            desc = createQRRequest.desc,
            checksum = _configuration["JsonStringVNPay:checksum"]!,
            billNumber = createQRRequest.billNumber,
            consumerId = "",
            purpose = ""
        };
        return data;
    }
    private Task<string> Caculate(string input)
    {
        return Task.Run(() =>
        {
            using (MD5 md5 = MD5.Create())
            {
                byte[] inputBytes = Encoding.ASCII.GetBytes(input);
                byte[] hashBytes = md5.ComputeHash(inputBytes);

                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < hashBytes.Length; i++)
                {
                    sb.Append(hashBytes[i].ToString("X2"));
                }
                return sb.ToString();
            }
        });
    }

}
