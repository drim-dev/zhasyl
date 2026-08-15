using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace Zhasyl.Api.Common.Errors;

public sealed class DomainException(
    int statusCode,
    string title,
    string detail,
    string errorCode) : Exception(detail)
{
    public int StatusCode { get; } = statusCode;
    public string Title { get; } = title;
    public string ErrorCode { get; } = errorCode;
}

public sealed class DomainExceptionHandler : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        if (exception is not DomainException domainException)
        {
            return false;
        }

        var problem = new ProblemDetails
        {
            Status = domainException.StatusCode,
            Title = domainException.Title,
            Detail = domainException.Message,
            Instance = httpContext.Request.Path,
        };
        problem.Extensions["errorCode"] = domainException.ErrorCode;
        problem.Extensions["traceId"] = httpContext.TraceIdentifier;
        httpContext.Response.StatusCode = domainException.StatusCode;
        await httpContext.Response.WriteAsJsonAsync(problem, cancellationToken);
        return true;
    }
}
