using MobileBankingUSSD.API.DTOs.Requests;
using MobileBankingUSSD.API.Helpers;
using MobileBankingUSSD.API.Models;
using MobileBankingUSSD.API.Repositories.interfaces;
using MobileBankingUSSD.API.Services.interfaces;

namespace MobileBankingUSSD.API.Services;

public class CustomerService(
    ICustomerRepository customerRepository,
    ILogger<CustomerService> logger) : ICustomerService
{
    private readonly ICustomerRepository _customerRepository =
        customerRepository ?? throw new ArgumentNullException(nameof(customerRepository));

    private readonly ILogger<CustomerService> _logger = logger ?? throw new ArgumentNullException(nameof(logger));


    public async Task<IEnumerable<Customer>> GetAllCustomersAsync(CancellationToken cancellationToken = default)
    {
        return await _customerRepository.GetAllCustomers(cancellationToken);
    }

    public async Task<Customer?> GetCustomerByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _customerRepository.GetCustomerById(id, cancellationToken);
    }

    public async Task<Customer?> GetCustomerByMsisdnAsync(string msisdn, CancellationToken cancellationToken = default)
    {
        if (!Validators.IsValidMsisdn(msisdn))
        {
            throw new ArgumentException("Invalid MSISDN format", nameof(msisdn));
        }

        return await _customerRepository.GetCustomerByMsisdn(msisdn, cancellationToken);

    }

    public async Task<Guid>  RegisterCustomerAsync(CustomerRegistrationRequest request,
        CancellationToken cancellationToken = default)
    {
        if (!Validators.IsValidMsisdn(request.Msisdn))
        {
            throw new ArgumentException("Invalid MSISDN format", nameof(request.Msisdn));
        }

        // checking msisdn is already registered
        if (await _customerRepository.GetCustomerByMsisdn(request.Msisdn, cancellationToken) != null)
        {
            throw new ArgumentException("MSISDN already registered");
        }
        
        // hash pin
        var pinHash = await _customerRepository.HashCustomerPin(request.Pin, cancellationToken);

        var customerRegistrationRequest = new CustomerRegistrationRequest
        {
            Msisdn = request.Msisdn,
            Pin = pinHash!,
            FullName = request.FullName
        };

       return await _customerRepository.RegisterCustomer(customerRegistrationRequest, cancellationToken);
    }

    public async Task<bool> VerifyCustomerPinAsync(VerifyPinRequest request,
        CancellationToken cancellationToken = default)
    {
        return await _customerRepository.VerifyPinFromDatabase(request, cancellationToken);
    }

    public async Task ChangePinAsync(ChangePinRequest request, CancellationToken cancellationToken = default)
    {
        var customer = await _customerRepository.GetCustomerById(request.CustomerId, cancellationToken);
        if (customer == null)
            throw new ArgumentException("Customer not found");

        // if (!customer.IsActive)
        //     throw new ArgumentException("Customer account is inactive");

        if (!await IsPinCorrect(customer.Msisdn, request.OldPin, cancellationToken))
            throw new ArgumentException("Current PIN is incorrect");

        if (!Validators.IsValidPin(request.NewPin))
            throw new ArgumentException("New PIN does not meet security requirements");
        
        var newPinHash = await _customerRepository.HashCustomerPin(request.NewPin, cancellationToken);
        if (newPinHash == null)
            throw new InvalidOperationException("Failed to hash new PIN");

        await _customerRepository.ChangePinFromDatabase(new ChangePinRequest
        {
            CustomerId = request.CustomerId,
            OldPin = request.OldPin,
            NewPin = newPinHash
        }, cancellationToken);

        _logger.LogInformation("PIN successfully changed for customer {CustomerId}", request.CustomerId);
      
    }

    private async Task<bool> IsPinCorrect(string msisdn, string pin, CancellationToken cancellationToken)
    {
        var result = await _customerRepository.VerifyPinFromDatabase(new VerifyPinRequest
        {
            Msisdn = msisdn,
            Pin = pin
        }, cancellationToken);

        return result != null;
    }

}