

using MobileBankingUSSD.API.DTOs.Requests;
using MobileBankingUSSD.API.DTOs.Responses;
using MobileBankingUSSD.API.Models;

namespace MobileBankingUSSD.API.Repositories.interfaces;

public interface ICustomerRepository
{
    Task<IEnumerable<Customer>> GetAllCustomers(CancellationToken cancellationToken = default);

    Task<Customer?> GetCustomerById(Guid id, CancellationToken cancellationToken = default);

    Task<Customer?> GetCustomerByMsisdn(string msisdn, CancellationToken cancellationToken = default);

    Task<string?> HashCustomerPin(string pin, CancellationToken cancellationToken = default);

    Task<Guid>  RegisterCustomer(CustomerRegistrationRequest request, CancellationToken cancellationToken = default);

    
    Task<bool> VerifyPinFromDatabase(VerifyPinRequest request,
        CancellationToken cancellationToken = default);


    Task ChangePinFromDatabase(ChangePinRequest request,
        CancellationToken cancellationToken = default);
}