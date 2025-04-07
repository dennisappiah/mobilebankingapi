using MobileBankingUSSD.API.DTOs.Requests;
using MobileBankingUSSD.API.DTOs.Responses;
using MobileBankingUSSD.API.Models;

namespace MobileBankingUSSD.API.Services.interfaces;

public interface ICustomerService
{
    Task<IEnumerable<Customer>> GetAllCustomersAsync(CancellationToken cancellationToken = default);
    Task<Customer?> GetCustomerByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<Customer?> GetCustomerByMsisdnAsync(string msisdn, CancellationToken cancellationToken = default);

    Task<Guid>  RegisterCustomerAsync(CustomerRegistrationRequest request,
        CancellationToken cancellationToken = default);

    Task<bool> VerifyCustomerPinAsync(VerifyPinRequest request, CancellationToken cancellationToken = default);

    Task ChangePinAsync(ChangePinRequest request, CancellationToken cancellationToken = default);
}