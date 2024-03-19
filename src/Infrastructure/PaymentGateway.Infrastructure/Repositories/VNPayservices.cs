using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using PaymentGateway.Domain.Common.ResponseBase;
using PaymentGateway.Domain.Repositories.VNPayRestful;
using PaymentGateway.Domain.Request;
using PaymentGateway.Domain.Response;
using PaymentGateway.Ultils.Extension;
using System.Text;
using System.Text.Json;

namespace PaymentGateway.Infrastructure.Repositories;

public class VnPayServices : IVnPayServices
{
    private readonly IConfiguration _configuration;
    private readonly Helpers _createQrCodeModel;
    private readonly HttpClient _httpClient;
    private readonly ILogger<VnPayServices> _logger;

    // ReSharper disable once ConvertToPrimaryConstructor
    public VnPayServices(HttpClient httpClient, IConfiguration configuration, Helpers createQrCodeModel,
        ILogger<VnPayServices> logger)
    {
        _httpClient = httpClient;
        _configuration = configuration;
        _createQrCodeModel = createQrCodeModel;
        _logger = logger;
    }

    public async Task<BaseResult> CreateQrString(CreateQrRequest createQrRequest)
    {
        var qrCodeModel = _createQrCodeModel.CreateQRRequestToCreateQR(createQrRequest);
        _logger.LogInformation("Create QRCodeModel success");
        var jsonString = JsonConvert.SerializeObject(qrCodeModel);
        var content = new StringContent(jsonString, Encoding.UTF8, "text/plain");
        var responseJsonString = await _httpClient.PostAsync(_configuration["ThirdParty:createQRUrl"], content);
        if (responseJsonString.IsSuccessStatusCode)
        {
            _logger.LogInformation("Send data to ThirdParty success");
            var response = await responseJsonString.Content.ReadAsStringAsync();
            var json = JsonDocument.Parse(response).RootElement;
            var jsonResponseData = JsonConvert.DeserializeObject<CreateQRStringResponse>(response)!;
            var code = json.GetProperty("code").GetString()!;
            _logger.LogInformation($"Code returned from the server is: {code}");
            var message = json.GetProperty("message").GetString()!;
            _logger.LogInformation($"Message returned from the server is: {message}");
            switch (code)
            {
                case "00":
                    //save data to database

                    return new BaseResult
                    {
                        IsSuccess = true,
                        Message = "Create QR success",
                        StatusCode = 200
                    };
                case "01":
                    return new BaseResult { IsSuccess = false, Message = message, StatusCode = 400 };
                case "04":
                    return new BaseResult { IsSuccess = false, Message = message, StatusCode = 500 };
                case "05":
                    return new BaseResult { IsSuccess = false, Message = message, StatusCode = 403 };
                case "06":
                    return new BaseResult { IsSuccess = false, Message = message, StatusCode = 400 };
                case "07":
                    return new BaseResult { IsSuccess = false, Message = message, StatusCode = 404 };
                case "09":
                    return new BaseResult { IsSuccess = false, Message = message, StatusCode = 400 };
                case "10":
                    return new BaseResult { IsSuccess = false, Message = message, StatusCode = 400 };
                case "11":
                    return new BaseResult { IsSuccess = false, Message = message, StatusCode = 403 };
                case "13":
                    return new BaseResult { IsSuccess = false, Message = message, StatusCode = 400 };
                case "15":
                    return new BaseResult { IsSuccess = false, Message = message, StatusCode = 400 };
                case "16":
                    return new BaseResult { IsSuccess = false, Message = message, StatusCode = 400 };
                case "21":
                    return new BaseResult { IsSuccess = false, Message = message, StatusCode = 400 };
                case "24":
                    return new BaseResult { IsSuccess = false, Message = message, StatusCode = 403 };
                case "99":
                    return new BaseResult { IsSuccess = false, Message = message, StatusCode = 500 };
                case "96":
                    return new BaseResult { IsSuccess = false, Message = message, StatusCode = 503 };
                default:
                    return new BaseResult
                    {
                        IsSuccess = false,
                        Message = "VNPay's payment system is having trouble, please choose another payment method",
                        StatusCode = 500
                    };
            }
        }

        return new BaseResult
        {
            IsSuccess = false,
            Message = "VNPay's payment system is having trouble, please choose another payment method"
        };
    }
}