using FluentResults;
using MediatR;
using Microsoft.Extensions.Logging;

namespace BuildFlow.Application.Abstractions.Behaviors;

// يسجّل بدء كل طلب ونتيجته
public sealed class LoggingBehavior<TRequest, TResponse>(
    ILogger<LoggingBehavior<TRequest, TResponse>> logger)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
    where TResponse : ResultBase
{
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        var requestName = typeof(TRequest).Name;

        logger.LogInformation(
            "Handling {RequestName}", requestName);

        var response = await next();

        if (response.IsSuccess)
        {
            logger.LogInformation(
                "Handled {RequestName} successfully", requestName);
        }
        else
        {
            var firstError = response.Errors.FirstOrDefault()?.Message;
            logger.LogWarning(
                "Request {RequestName} failed: {Error}",
                requestName, firstError);
        }

        return response;
    }
}