namespace MobileBankingUSSD.API.Exceptions;

public class CustomerNotFoundException : NotFoundException
{
    public Guid CustomerId { get; }

    public  CustomerNotFoundException(Guid customerId, string? message = null) 
        : base(message ?? $"Customer with ID '{customerId}' was not found.")
    {
        CustomerId = customerId;
    }
}