using MobileBankingUSSD.API.DTOs.Responses;
using MobileBankingUSSD.API.Enums;


namespace MobileBankingUSSD.API.Repositories.interfaces;

public interface IAccountRepository
{
    Task CreateAccount(Guid customerId, string accountNumber, AccountType accountType, decimal initialBalance, bool isActive,
        CancellationToken cancellationToken);

    Task<AccountBalanceResponse> GetAccountBalanceAsync(string accountNumber, CancellationToken cancellationToken);
    
    Task<bool> AccountNumberExistsAsync(string accountNumber, CancellationToken cancellationToken);

    Task<(Guid Id, string AccountNumber, decimal Balance, decimal AvailableBalance, bool IsActive, DateTime LastUpdated)
        ?> GetAccountByNumber(string accountNumber, CancellationToken cancellationToken);
}