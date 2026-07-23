using BuildFlow.Documents.Domain.Entities;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace BuildFlow.Documents.Infrastructure.Persistence.Converters;

public sealed class DocumentIdConverter
    : ValueConverter<DocumentId, Guid>
{
    public DocumentIdConverter()
        : base(id => id.Value, value => new DocumentId(value))
    {
    }
}

public sealed class DocumentVersionIdConverter
    : ValueConverter<DocumentVersionId, Guid>
{
    public DocumentVersionIdConverter()
        : base(id => id.Value, value => new DocumentVersionId(value))
    {
    }
}