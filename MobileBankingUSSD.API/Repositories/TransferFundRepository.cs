using System.Data;
using Dapper;
using MobileBankingUSSD.API.Data.DbService;
using MobileBankingUSSD.API.Repositories.interfaces;
using MobileBankingUSSD.API.Services;

namespace MobileBankingUSSD.API.Repositories;

public class TransferFundRepository(
    DbTransactionFactory databaseTransactionFactory,
    ILogger<TransferFundService> logger)
    : ITransferFundRepository
{
    private readonly DbTransactionFactory _dbTransactionFactory = databaseTransactionFactory ?? throw new ArgumentNullException(nameof(databaseTransactionFactory));
    private readonly ILogger<TransferFundService> _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    private const string Schema = "public";

    public async Task<Guid> TransferFundsAsync(Guid fromAccountId, Guid toAccountId, decimal amount, string reference, CancellationToken cancellationToken)
    {
        var parameters = new DynamicParameters();
        parameters.Add("p_from_account_id", fromAccountId);
        parameters.Add("p_to_account_id", toAccountId);
        parameters.Add("p_amount", amount);
        parameters.Add("p_reference", reference);
        parameters.Add("p_transaction_id", dbType: DbType.Guid, direction: ParameterDirection.Output);
        parameters.Add("p_success", dbType: DbType.Boolean, direction: ParameterDirection.Output);
        parameters.Add("p_message", dbType: DbType.String, direction: ParameterDirection.Output);

        await _dbTransactionFactory.ExecuteStoredProcedureWithOutputAsync<Guid>(
            Schema,
            "transfer_funds",
            parameters,
            "p_transaction_id",
            cancellationToken
        );

        if (!parameters.Get<bool>("p_success"))
        {
            throw new Exception(parameters.Get<string>("p_message"));
        }

        return parameters.Get<Guid>("p_transaction_id");
    }
}