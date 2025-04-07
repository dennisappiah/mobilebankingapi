
using Dapper;
using MobileBankingUSSD.API.Data.DbService;
using MobileBankingUSSD.API.DTOs.Responses;
using MobileBankingUSSD.API.Enums;
using MobileBankingUSSD.API.Repositories.interfaces;
using Npgsql;

namespace MobileBankingUSSD.API.Repositories;

public class AccountRepository(DbTransactionFactory dbTransactionFactory): IAccountRepository
{
    private readonly DbTransactionFactory _dbTransactionFactory = dbTransactionFactory ?? throw new ArgumentNullException(nameof(dbTransactionFactory));
    private const string Schema = "public";
    
    public async Task CreateAccount(Guid customerId, string accountNumber, AccountType accountType, decimal initialBalance,
        bool isActive, CancellationToken cancellationToken)
    {
        var accountTypeId = await GetAccountTypeId(accountType, cancellationToken);
        
        var parameters = new DynamicParameters();
        parameters.Add("p_customer_id", customerId);
        parameters.Add("p_account_number", accountNumber);
        parameters.Add("p_account_type", accountTypeId);
        parameters.Add("p_initial_balance", initialBalance);
        parameters.Add("p_is_active", isActive);
        
        await _dbTransactionFactory.ExecuteAsync(
            Schema,
            "create_account", 
            parameters,
            cancellationToken,
            isStoredProcedure: true
        );
    }

    public async Task<AccountBalanceResponse> GetAccountBalanceAsync(string accountNumber, CancellationToken cancellationToken)
    {
        var parameters = new { p_account_number = accountNumber };

        try
        {
            var result = await _dbTransactionFactory.QuerySingleAsync<AccountBalanceResponse>(
                Schema,
                "get_account_balance",
                parameters,
                cancellationToken
            );

            return result ?? throw new KeyNotFoundException($"Account {accountNumber} not found");
        }
        catch (PostgresException ex) when (ex.SqlState == "P0001")
        {
            throw new KeyNotFoundException(ex.Message);
        }
    }


    public async Task<bool> AccountNumberExistsAsync(string accountNumber, CancellationToken cancellationToken)
    {
        return await _dbTransactionFactory.QuerySingleAsync<bool>(
            Schema,
            "account_number_exists",
            new { account_number_param = accountNumber }, cancellationToken
        );
    }

    public Task<(Guid Id, string AccountNumber, decimal Balance, decimal AvailableBalance, bool IsActive, DateTime LastUpdated)?> GetAccountByNumber(string accountNumber, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }


    private async Task<int> GetAccountTypeId(AccountType accountType, CancellationToken cancellationToken)
    {
        return await _dbTransactionFactory.QuerySingleAsync<int>(
            Schema,
            "get_account_type_id",
            new { p_account_type_name = accountType.ToString() }, cancellationToken
        );
    }
    
    
    

}