using System.Text.RegularExpressions;
using BuildFlow.SharedKernel.Domain;

namespace BuildFlow.Projects.Domain.ValueObjects;

/// <summary>Unique project code within a tenant, e.g. "PROJ-2024-001"</summary>
public class ProjectCode : ValueObject
{
    private static readonly Regex CodeRegex =
        new(@"^[A-Z0-9\-]{3,20}$", RegexOptions.Compiled);

    public string Value { get; }

    private ProjectCode(string value) => Value = value;

    public static ProjectCode Create(string code)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(code);
        var upper = code.Trim().ToUpperInvariant();
        if (!CodeRegex.IsMatch(upper))
            throw new ArgumentException($"'{code}' is not a valid project code. Use uppercase letters, numbers and hyphens (3-20 chars).");
        return new ProjectCode(upper);
    }

    protected override IEnumerable<object?> GetEqualityComponents() { yield return Value; }
    public override string ToString() => Value;
    public static implicit operator string(ProjectCode c) => c.Value;
}