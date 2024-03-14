using Microsoft.AspNetCore.Mvc;
using PaymentGateway.Ultils.Extension;

namespace PaymentGateway.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class TestsController : ControllerBase
{
    private readonly Helpers _helpers;

    public TestsController(Helpers helpers)
    {
        _helpers = helpers;
    }

    [HttpGet]
    public async Task<IActionResult> TestAction()
    {
        var productId = "";
        var txnId = "ND03427/0618";
        var amount = "1";
        var tipAndFee = "1";
        var data = await _helpers.CalculateMD5GenQR(productId, txnId, amount, tipAndFee);
        return Ok(data);
    }
}