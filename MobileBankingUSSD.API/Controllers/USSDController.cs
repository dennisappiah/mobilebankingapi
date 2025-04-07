using Microsoft.AspNetCore.Mvc;
using MobileBankingUSSD.API.DTOs.Requests;
using MobileBankingUSSD.API.DTOs.Responses;
using MobileBankingUSSD.API.Services.interfaces;

namespace MobileBankingUSSD.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UssdController(IUssdService ussdService) : ControllerBase
{
    [HttpGet] 
    public async Task<IActionResult> ProcessUssdRequest(
        [FromQuery] string sessionId,
        [FromQuery] string msisdn,
        [FromQuery] string serviceCode,
        [FromQuery] string NetworkCode,
        [FromQuery] string text
    )
    {
        try
        {
            
            var request = new UssdRequest
            {
                SessionId = sessionId,
                Msisdn = msisdn,
                ServiceCode = serviceCode,
                NetworkCode = NetworkCode,
                Text = text
                
            };

            var response = await ussdService.ProcessRequest(request);
            return Ok(response);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new UssdResponse
            {
                Response = "An error occurred. Please try again later.",
                EndSession = true
            });
        }
    }
}