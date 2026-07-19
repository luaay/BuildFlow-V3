using FluentResults;
using FluentValidation;
using MediatR;

namespace BuildFlow.Application.Abstractions.Behaviors;

// يعترض كل أمر، ويتحقّق منه قبل بلوغ معالجه
public sealed class ValidationBehavior<TRequest, TResponse>(
    IEnumerable<IValidator<TRequest>> validators)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
    where TResponse : ResultBase, new()
{
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        // لا متحقّق Validator لهذا الطلب، فمرّره مباشرةً
        if (!validators.Any())
            return await next();

        var context = new ValidationContext<TRequest>(request);

        // شغّل كل متحقّقات هذا الطلب
        var results = await Task.WhenAll(
            validators.Select(v => v.ValidateAsync(context, cancellationToken)));

        var failures = results
            .SelectMany(r => r.Errors)
            .Where(f => f is not null)
            .ToList();

        // نجح التحقّق، فمرّر الطلب إلى معالجه Handler
        if (failures.Count == 0)
            return await next();

        // فشل التحقّق: ابنِ نتيجة فاشلة، بلا بلوغ المعالج
        var errors = failures
            .Select(f => new Error(f.ErrorMessage)
                .WithMetadata("Code", $"Validation.{f.PropertyName}"))
            .ToList();

        var response = new TResponse();
        response.Reasons.AddRange(errors);
        return response;
    }
}