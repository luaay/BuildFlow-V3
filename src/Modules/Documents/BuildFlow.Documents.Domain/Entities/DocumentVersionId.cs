namespace BuildFlow.Documents.Domain.Entities;

public readonly record struct DocumentVersionId(Guid Value)
{
    public static DocumentVersionId New() => new(Guid.NewGuid());
    public override string ToString() => Value.ToString();
}