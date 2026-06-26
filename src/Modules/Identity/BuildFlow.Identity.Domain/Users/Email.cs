using System.Text.RegularExpressions;
using BuildFlow.SharedKernel.Domain;
using FluentResults;

namespace BuildFlow.Identity.Domain.Users;

// Email value object — أي Email موجود = بريد صالح بالضرورة
public sealed class Email : ValueObject
{
    // regex مُجمّع يُنشأ مرة واحدة (static readonly) — أداء جيد وموثوق
    private static readonly Regex EmailRegex =
        new(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.Compiled);

    public string Value { get; }

    // private constructor — الإنشاء فقط عبر Create() بعد التحقّق
    private Email(string value)
    {
        Value = value;
    }

    public static Result<Email> Create(string? email)
    {
        if (string.IsNullOrWhiteSpace(email))
            return Result.Fail<Email>("Email is required.");

        var normalized = email.Trim().ToLowerInvariant();

        if (!EmailRegex.IsMatch(normalized))
            return Result.Fail<Email>("Email format is invalid.");

        return Result.Ok(new Email(normalized));
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }

    public override string ToString() => Value;
}