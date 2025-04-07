
using MobileBankingUSSD.API.DTOs.Requests;
using MobileBankingUSSD.API.DTOs.Responses;

namespace MobileBankingUSSD.API.Services.interfaces;

public interface IAccountService
{
    Task CreateAccount(CreateAccountRequest createAccountRequest, CancellationToken cancellationToken);

    Task<AccountBalanceResponse> GetAccountBalanceAsync(string accountNumber, CancellationToken cancellationToken);
    
    Task<(Guid Id, string AccountNumber, decimal Balance, decimal AvailableBalance, bool IsActive, DateTime LastUpdated)
        ?> GetAccountByNumber(string accountNumber, CancellationToken cancellationToken);
}