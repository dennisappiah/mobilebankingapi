
using MobileBankingUSSD.API.DTOs.Requests;
using MobileBankingUSSD.API.DTOs.Responses;
using MobileBankingUSSD.API.Repositories.interfaces;
using MobileBankingUSSD.API.Services.interfaces;

namespace MobileBankingUSSD.API.Services;

 public class AccountService(IAccountRepository accountRepository, ILogger<AccountService> logger)
     : IAccountService
 {
        private readonly IAccountRepository _accountRepository = accountRepository ?? throw new ArgumentNullException(nameof(accountRepository));
        private readonly ILogger<AccountService> _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        private readonly Random _random = new Random();
        public async Task CreateAccount(CreateAccountRequest accountRequest, CancellationToken cancellationToken)
        {

            // Generate unique account number
            var accountNumber = await GenerateUniqueAccountNumber(cancellationToken);

            _logger.LogInformation($"Creating account with account number: {accountNumber}");
            try
            {
                await _accountRepository.CreateAccount(accountRequest.CustomerId, accountNumber, accountRequest.AccountType, accountRequest.InitialBalance, accountRequest.isActive, cancellationToken);
                _logger.LogInformation($"Account {accountNumber} created successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error creating account {accountNumber}.");
                throw;
            }
        }

        public async Task<AccountBalanceResponse> GetAccountBalanceAsync(string accountNumber, CancellationToken cancellationToken)
        {
            var result = await _accountRepository.GetAccountBalanceAsync(accountNumber, cancellationToken);

            _logger.LogInformation($"Account balance retrieved for {accountNumber}");
            return result;
        }

        public async Task<(Guid Id, string AccountNumber, decimal Balance, decimal AvailableBalance, bool IsActive, DateTime LastUpdated)?> GetAccountByNumber(string accountNumber, CancellationToken cancellationToken)
        {
            return await _accountRepository.GetAccountByNumber(accountNumber, cancellationToken);
        }

        private async Task<string>GenerateUniqueAccountNumber(CancellationToken cancellationToken)
        {
            string accountNumber;
            do
            {
                accountNumber = Generate10DigitAccountNumber();
            } while (await _accountRepository.AccountNumberExistsAsync(accountNumber, cancellationToken));

            // Add Validation here.
            if (accountNumber.Length != 10 || !accountNumber.All(char.IsDigit))
            {
                _logger.LogError($"Generated invalid account number: {accountNumber}");
                throw new InvalidOperationException("Generated invalid account number.");
            }

            return accountNumber;
        }

        private string Generate10DigitAccountNumber()
        {
            return _random.Next(1_000_000_000, 2_000_000_000).ToString();
        }

       
    }

    
