using Microsoft.AspNetCore.Mvc;
using PaymentGateway.Ultils.Extension;

namespace PaymentGateway.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        public WeatherForecastController(Helpers helpers, ILogger<WeatherForecastController> logger)
        {
            _helpers = helpers;
            _logger = logger;

        }
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<WeatherForecastController> _logger;
        private readonly Helpers _helpers;
        [HttpGet(Name = "GetWeatherForecast")]
        public async Task<IEnumerable<WeatherForecast>> Get()
        {
            var merchantName = "POS365";
            var merchantCode = "0106913377";
            var terminalId = "POS01";
            var productId = "";
            var txnId = "ND03427/0618";
            var amount = "1";
            var tipAndFee = "";

            var data = await _helpers.CalculateMD5(merchantName, merchantCode, terminalId, productId, txnId, amount, tipAndFee);

            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            })
            .ToArray();
        }

    }
}
