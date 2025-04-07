namespace MobileBankingUSSD.API.Exceptions;

public class CustomerRegistrationException(string message, ErrorDetails[]? errors = null): Exception(message)
{
    public ErrorDetails[]? Errors { get; set; } = errors ?? [];
}