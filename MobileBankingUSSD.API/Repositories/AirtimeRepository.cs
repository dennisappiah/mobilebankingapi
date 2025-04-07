using System.Data;
using Dapper;
using MobileBankingUSSD.API.Data.DbService;

namespace MobileBankingUSSD.API.Repositories;

public class AirtimeRepository(DbTransactionFactory dbTransactionFactory)
{
    private readonly DbTransactionFactory _dbTransactionFactory = dbTransactionFactory ?? throw new ArgumentNullException(nameof(dbTransactionFactory));
    private const string Schema = "public";
    
    
    public async Task<Guid> PurchaseAirtimeAsync(string accountNumber, string phoneNumber, string network, decimal amount, string reference, CancellationToken cancellationToken)
    {
        var parameters = new DynamicParameters();
        parameters.Add("p_account_number", accountNumber);
        parameters.Add("p_phone_number", phoneNumber);
        parameters.Add("p_network", network);
        parameters.Add("p_amount", amount);
        parameters.Add("p_reference", reference);
        parameters.Add("p_transaction_id", dbType: DbType.Guid , direction:ParameterDirection.Output);
        parameters.Add("p_airtime_purchase_id", dbType: DbType.Guid, direction: ParameterDirection.Output);
        parameters.Add("p_success", dbType: DbType.Boolean, direction: ParameterDirection.Output);
        parameters.Add("p_message", dbType: DbType.String, direction: ParameterDirection.Output);
        
        await _dbTransactionFactory.ExecuteAsync(
            Schema,
            "", 
            parameters,
            cancellationToken
        );


        if (!parameters.Get<bool>("p_success"))
        {
            throw new Exception(parameters.Get<string>("p_message"));
        }

        return parameters.Get<Guid>("p_transaction_id");
    }
    
    
}