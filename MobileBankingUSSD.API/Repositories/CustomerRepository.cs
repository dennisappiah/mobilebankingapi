using System.Data;
using Dapper;
using MobileBankingUSSD.API.Data.DbService;
using MobileBankingUSSD.API.DTOs.Requests;
using MobileBankingUSSD.API.Models;
using MobileBankingUSSD.API.Repositories.interfaces;


namespace MobileBankingUSSD.API.Repositories;

public class CustomerRepository(DbTransactionFactory dbTransactionFactory): ICustomerRepository
{
    private readonly DbTransactionFactory _dbTransactionFactory = dbTransactionFactory ?? throw new ArgumentNullException(nameof(dbTransactionFactory));
    private const string Schema = "public";
    
    public async Task<IEnumerable<Customer>> GetAllCustomers(CancellationToken cancellationToken = default)
    {
        return await _dbTransactionFactory.QueryListAsync<Customer>(
            Schema,
            "get_all_customers",
            cancellationToken: cancellationToken);
    }

    public async Task<Customer?> GetCustomerById(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbTransactionFactory.QuerySingleAsync<Customer>(
            Schema,
            "get_customer_by_id",
            new { p_id = id },
            cancellationToken);
    }

    public async Task<Customer?> GetCustomerByMsisdn(string msisdn, CancellationToken cancellationToken = default)
    {
        return await _dbTransactionFactory.QuerySingleAsync<Customer>(
            Schema,
            "retrieve_customer_by_msisdn",
            new { p_msisdn = msisdn },
            cancellationToken);
    }
    
    public async Task<string?> HashCustomerPin(string pin, CancellationToken cancellationToken = default)
    {
        return await _dbTransactionFactory.QuerySingleAsync<string>(
            Schema,
            "hash_customer_pin",
            new { p_pin = pin },
            cancellationToken);
    }

    public async Task<Guid> RegisterCustomer(CustomerRegistrationRequest request, CancellationToken cancellationToken = default)
    {
        var parameters = new DynamicParameters();
        parameters.Add("p_msisdn", request.Msisdn);
        parameters.Add("p_pin_hash", request.Pin);
        parameters.Add("p_full_name", request.FullName);
        parameters.Add("p_customer_id", dbType: DbType.Guid, direction: ParameterDirection.Output);

        return await _dbTransactionFactory.ExecuteStoredProcedureWithOutputAsync<Guid>(
            Schema,
            "create_customer",
            parameters,
            "p_customer_id",
            cancellationToken
        );
    }

    public async Task<bool> VerifyPinFromDatabase(VerifyPinRequest request,
        CancellationToken cancellationToken = default)
    {
       return await _dbTransactionFactory.QuerySingleAsync<bool>(
            Schema,
            "verify_customer_pin",
            new { p_msisdn = request.Msisdn, 
                p_pin = request.Pin },
            cancellationToken
        );
    }
    
   
    public async Task ChangePinFromDatabase(ChangePinRequest request, 
        CancellationToken cancellationToken = default)
    {
        var parameters = new DynamicParameters();
        parameters.Add("p_customer_id", request.CustomerId);
        parameters.Add("p_new_pin_hash",   request.NewPin); 
        
        await _dbTransactionFactory.ExecuteAsync(
            Schema,
            "change_customer_pin", 
            parameters,
            cancellationToken,
            isStoredProcedure: true
        );

    }
    
}