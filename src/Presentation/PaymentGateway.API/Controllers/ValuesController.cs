using MediatR;
using Microsoft.AspNetCore.Mvc;
using PaymentGateway.Application.Features.Merchants.Queries;
using PaymentGateway.Domain.Entities;
using PaymentGateway.Ultils.ConfigDBConnection.Impl;

namespace PaymentGateway.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        private readonly ISender _sender;
        private readonly IDataAccess _data;

        public ValuesController(ISender sender, IDataAccess data)
        {
            _sender = sender;
            _data = data;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var result = await _sender.Send(new GetAllMerchantsQuery());
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Post()
        {
            var merchant = new Merchant
            {
                Id = 0,
                MerchantId = "M" + Guid.NewGuid(),
                CreatedAt = DateTime.Now,
                MerchantName = "createMerchant.MerchantName",
                CreatedBy = "createMerchant.CreatedBy",
                IsActive = true,
                Deleted = false
            };
            var result = await _data.InsertData("Merchant", merchant);
            return Ok(result);
        }
        [HttpPut("/{MerchantId}")]
        public async Task<IActionResult> Update(string MerchantId)
        {
            var merchant = new UpdateNameMerchantModel()
            {
                //LastUpdatedAt = DateTime.Now,
                LastUpdatedBy = "update test",
                MerchantName = "update",
            };
            var result = await _data.UpdateData("Merchant", MerchantId, merchant);
            return Ok(result);
        }
    }
}
