using MediatR;
using Microsoft.AspNetCore.Mvc;
using PaymentGateway.Application.Features.Merchants.Queries;

namespace PaymentGateway.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        private readonly ISender _sender;

        public ValuesController(ISender sender)
        {
            _sender = sender;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var result = await _sender.Send(new GetAllMerchantsQuery());
            return Ok(result);
        }
    }
}
