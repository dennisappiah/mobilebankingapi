namespace MobileBankingUSSD.API.Exceptions;

public class BadRequestException(string message, ErrorDetails[]? errors = null) : Exception(message)
{
    public ErrorDetails[]? Errors { get; set; } = errors ?? [];
}

