﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace PaymentGateway.API.Controllers.Base
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class PaymentGatewayVersion : ControllerBase
    {
    }
}
