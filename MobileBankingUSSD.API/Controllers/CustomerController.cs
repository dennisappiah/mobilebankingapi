using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Mvc;
using MobileBankingUSSD.API.DTOs.Requests;
using MobileBankingUSSD.API.DTOs.Responses;
using MobileBankingUSSD.API.Services.interfaces;

namespace MobileBankingUSSD.API.Controllers;

[ApiController]
[Route("api/customers")]
public partial class CustomerController(ICustomerService customerService, ILogger<CustomerController> logger) : ControllerBase
{
    
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<CustomerDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<CustomerDto>>> GetAllCustomers()
    {
        logger.LogInformation("Fetching all customers");
        var customers = await customerService.GetAllCustomersAsync();
        return Ok(customers);
    }

    [HttpGet("msisdn/{msisdn}")]
    [ProducesResponseType(typeof(CustomerDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CustomerDto>> GetCustomerByMsisdn(string msisdn)
    {
        logger.LogInformation("Fetching customer with MSISDN: {MSISDN}", msisdn);
        var customer = await customerService.GetCustomerByMsisdnAsync(msisdn);
        return Ok(customer);
    }

    [HttpGet("{id}", Name = "GetCustomerById")]
    [ProducesResponseType(typeof(CustomerDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CustomerDto>> GetCustomerById(Guid id) 
    {
        logger.LogInformation("Fetching customer with ID: {ID}", id);

        var customer = await customerService.GetCustomerByIdAsync(id);

        return Ok(customer);
    }

    [HttpPost("register")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<Guid>> RegisterCustomer([FromBody] CustomerRegistrationRequest request)
    {
        
        return await customerService.RegisterCustomerAsync(request);

    }

    [HttpPost("verify-pin")]
    [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<bool>> VerifyCustomerRegistration([FromBody] VerifyPinRequest pinRequest)
    {
        if (!MsisdnRegex().IsMatch(pinRequest.Msisdn))
        {
            return BadRequest("Invalid MSISDN format. Must be 12 digits, and start with 233.");
        }
    
        var isPinValid = await customerService.VerifyCustomerPinAsync(pinRequest);
        return Ok(isPinValid);
    }
    
    
    [HttpPost("change-pin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> ChangeCustomerPin([FromBody] ChangePinRequest request)
    {
        
        await customerService.ChangePinAsync(request);
        return NoContent();
    }
    
    [GeneratedRegex("^233[0-9]{9}$")]
    private static partial Regex MsisdnRegex();
}