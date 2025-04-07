using Microsoft.AspNetCore.Mvc;
using MobileBankingUSSD.API.DTOs.Requests;
using MobileBankingUSSD.API.DTOs.Responses;
using MobileBankingUSSD.API.Services.interfaces;

namespace MobileBankingUSSD.API.Controllers;

[ApiController]
[Route("api/accounts")]
public class AccountController(IAccountService accountService, ILogger<AccountController> logger): ControllerBase
{
    private readonly IAccountService _accountService = accountService ?? throw new ArgumentNullException(nameof(accountService));
    private readonly ILogger<AccountController> _logger = logger ?? throw new ArgumentNullException(nameof(logger));


        [HttpPost("create")]
        [ProducesResponseType(typeof(CustomerDto), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task CreateAccountAsync(CreateAccountRequest createAccountRequest,
            CancellationToken cancellationToken = default)
        {
            await _accountService.CreateAccount(createAccountRequest, cancellationToken);
        }
    
    
        [HttpGet("{accountNumber}/balance", Name = "GetAccountBalance")]
        [ProducesResponseType(typeof(AccountBalanceResponse), 200)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetBalance(string accountNumber, CancellationToken cancellationToken = default)
        {
            
            _logger.LogInformation("Balance inquiry requested for account: {AccountNumber}", accountNumber);
                
            var balance = await _accountService.GetAccountBalanceAsync(accountNumber, cancellationToken);
                
            _logger.LogInformation("Successfully retrieved balance for account: {AccountNumber}", accountNumber);
                
            return Ok(balance);
        }
}

