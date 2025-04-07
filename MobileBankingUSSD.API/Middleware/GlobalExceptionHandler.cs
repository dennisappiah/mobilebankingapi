using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using MobileBankingUSSD.API.Exceptions;

namespace MobileBankingUSSD.API.Middleware;

public sealed class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        ProblemDetails problemDetails;

        switch (exception)
        {
            case CustomerNotFoundException cnf:
                problemDetails = new ProblemDetails
                {
                    Title = "Customer not found",
                    Status = StatusCodes.Status404NotFound,
                    Detail = cnf.Message,
                    Type = "https://httpstatuses.com/404",
                };
                logger.LogWarning(cnf, "Customer not found.");
                break;

            case BadRequestException br:
                problemDetails = new ProblemDetails
                {
                    Title = "Bad request",
                    Status = StatusCodes.Status400BadRequest,
                    Detail = br.Message,
                    Type = "https://httpstatuses.com/400",
                    Extensions = { ["errors"] = br.Errors }
                };
                logger.LogWarning(br, "Bad request.");
                break;

            default:
                problemDetails = new ProblemDetails
                {
                    Title = "Server Error",
                    Status = StatusCodes.Status500InternalServerError,
                    Detail = "An unexpected error occurred.",
                    Type = "https://httpstatuses.com/500",
                };
                logger.LogError(exception, "Unhandled exception.");
                break;
        }

        if (problemDetails.Status != null)
        {
            httpContext.Response.StatusCode = problemDetails.Status.Value;
        }

        await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);

        return true;
    }
}