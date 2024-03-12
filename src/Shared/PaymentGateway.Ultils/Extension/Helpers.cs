using Microsoft.Extensions.Configuration;
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
