using BuildFlow.Identity.Domain.Tenants;
using BuildFlow.Identity.Domain.Users;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace BuildFlow.Identity.Infrastructure.Persistence.Converters;

// محوّل TenantId ↔ Guid
public sealed class TenantIdConverter : ValueConverter<TenantId, Guid>
{
    public TenantIdConverter()
        : base(
            id => id.Value,              // عند الحفظ: TenantId → Guid
            value => new TenantId(value)) // عند القراءة: Guid → TenantId
    {
    }
}

// محوّل UserId ↔ Guid
public sealed class UserIdConverter : ValueConverter<UserId, Guid>
{
    public UserIdConverter()
        : base(
            id => id.Value,            // عند الحفظ: UserId → Guid
            value => new UserId(value)) // عند القراءة: Guid → UserId
    {
    }
}