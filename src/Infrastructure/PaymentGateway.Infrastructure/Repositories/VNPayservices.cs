using Microsoft.Extensions.Configuration;
using PaymentGateway.Domain.Common.ResponseModel;
using PaymentGateway.Domain.Entities.ThirdParty;
using PaymentGateway.Domain.Repositories;
using System.Text;

namespace PaymentGateway.Infrastructure.Repositories;
public class VNPayservices : IVNPayservices
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;

    public VNPayservices(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _configuration = configuration;
    }
    public async Task<BaseResult> CreateQRString(CreateQR createQR)
    {
        var content = new StringContent(createQR.ToString()!, Encoding.UTF8, "text/plain");
        var request = await _httpClient.PostAsync(_configuration["ThirdParty:createQRUrl"], content);


        throw new NotImplementedException();
    }
}
