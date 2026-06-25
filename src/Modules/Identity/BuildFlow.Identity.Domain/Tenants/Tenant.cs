using BuildFlow.Identity.Domain.Tenants.Enums;
using BuildFlow.Identity.Domain.Tenants.Events;
using BuildFlow.SharedKernel.Domain;
using BuildFlow.SharedKernel.Domain.Auditing;

namespace BuildFlow.Identity.Domain.Tenants;

public sealed class Tenant : AggregateRoot<TenantId>, IAuditableEntity, ISoftDelete
{
    public string Name { get; private set; } = null!;
    public string Slug { get; private set; } = null!;
    public TenantStatus Status { get; private set; }
    public TenantPlan Plan { get; private set; }
    public DateTime? SuspendedAtUtc { get; private set; }

    // IAuditableEntity
    public DateTime CreatedAtUtc { get; set; }
    public Guid CreatedBy { get; set; }
    public DateTime? ModifiedAtUtc { get; set; }
    public Guid? ModifiedBy { get; set; }

    // ISoftDelete
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAtUtc { get; set; }
    public Guid? DeletedBy { get; set; }

    // private constructor — لـ EF Core فقط، يمنع الإنشاء المباشر
    private Tenant() { }

    // Factory Method — الطريقة الوحيدة لإنشاء Tenant
    public static Tenant Create(string name, string slug, TenantPlan plan)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        ArgumentException.ThrowIfNullOrWhiteSpace(slug);

        var tenant = new Tenant
        {
            Id = TenantId.New(),
            Name = name.Trim(),
            Slug = slug.Trim().ToLowerInvariant(),
            Status = TenantStatus.Active,
            Plan = plan
        };

        tenant.RaiseDomainEvent(new TenantCreatedEvent(tenant.Id, tenant.Name));
        return tenant;
    }

    public void Suspend()
    {
        if (Status == TenantStatus.Suspended)
            return; // idempotent

        Status = TenantStatus.Suspended;
        SuspendedAtUtc = DateTime.UtcNow;
        RaiseDomainEvent(new TenantSuspendedEvent(Id));
    }

    public void Activate()
    {
        Status = TenantStatus.Active;
        SuspendedAtUtc = null;
    }

    public void UpdatePlan(TenantPlan newPlan)
    {
        Plan = newPlan;
    }
}