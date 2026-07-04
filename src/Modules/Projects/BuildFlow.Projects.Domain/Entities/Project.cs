using BuildFlow.SharedKernel.Domain;
using BuildFlow.SharedKernel.Domain.Auditing;
using BuildFlow.Projects.Domain.Enums;
using BuildFlow.Projects.Domain.Events;
using BuildFlow.Projects.Domain.ValueObjects;
using FluentResults;
using BuildFlow.Projects.Domain.Errors;

namespace BuildFlow.Projects.Domain.Entities;

public sealed class Project : AggregateRoot<ProjectId>, IAuditableEntity, ISoftDelete

{
    // Raw Guid on the boundary toward the Identity module.
    public Guid TenantId { get; private set; }

    public string Name { get; private set; } = null!;
    public ProjectCode Code { get; private set; } = null!;
    public string? Description { get; private set; }
    public ProjectStatus Status { get; private set; }
    public Money Budget { get; private set; } = null!;
    public DateTime? StartDate { get; private set; }
    public DateTime? EndDate { get; private set; }
    public string? ClientName { get; private set; }
    public string? Location { get; private set; }

    // Auditing as direct properties (consistent with the Identity module).
   // IAuditableEntity — filled manually on create/update.
    public DateTime CreatedAtUtc { get; set; }
    public Guid CreatedBy { get; set; }
    public DateTime? ModifiedAtUtc { get; set; }
    public Guid? ModifiedBy { get; set; }

    // ISoftDelete — flagged, not physically removed.
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAtUtc { get; set; }
    public Guid? DeletedBy { get; set; }

    private readonly List<ProjectMember> _members = [];
    public IReadOnlyList<ProjectMember> Members => _members.AsReadOnly();

    // EF Core constructor.
    private Project() : base() { }

    private Project(ProjectId id) : base(id) { }

    // ── Factory ───────────────────────────────────────────────
    public static Result<Project> Create(
        Guid tenantId,
        string name,
        string code,
        string? description,
        decimal budget,
        string currency,
        Guid createdByUserId,
        string? clientName = null,
        string? location = null,
        DateTime? startDate = null,
        DateTime? endDate = null)
    {
        if (string.IsNullOrWhiteSpace(name))
            return Result.Fail(ProjectErrors.NameRequired);

        if (endDate.HasValue && startDate.HasValue && endDate < startDate)
            return Result.Fail(ProjectErrors.EndDateBeforeStart);

        var project = new Project(ProjectId.New())
        {
            TenantId     = tenantId,
            Name         = name.Trim(),
            Code         = ProjectCode.Create(code),
            Description  = description?.Trim(),
            Status       = ProjectStatus.Planning,
            Budget       = Money.Create(budget, currency),
            ClientName   = clientName?.Trim(),
            Location     = location?.Trim(),
            StartDate    = startDate,
            EndDate      = endDate,
            CreatedAtUtc = DateTime.UtcNow,
            CreatedBy    = createdByUserId
        };

        // Creator automatically becomes the Lead (added in Part 2 logic).
        project._members.Add(
            ProjectMember.Create(project.Id, createdByUserId, ProjectMemberRole.Lead));

        project.RaiseDomainEvent(
            new ProjectCreatedEvent(project.Id.Value, tenantId, project.Name, createdByUserId));

        return Result.Ok(project);
    }

    // ── Lifecycle (Result pattern) ────────────────────────────
    public Result Activate()
    {
        if (Status is not (ProjectStatus.Planning or ProjectStatus.OnHold))
            return Result.Fail(
                ProjectErrors.InvalidStatusTransition(Status.ToString(), nameof(ProjectStatus.Active)));

        Status = ProjectStatus.Active;
        Touch();
        RaiseDomainEvent(new ProjectStatusChangedEvent(Id.Value, TenantId, ProjectStatus.Active));
        return Result.Ok();
    }

    public Result PutOnHold()
    {
        if (Status != ProjectStatus.Active)
            return Result.Fail(
                ProjectErrors.InvalidStatusTransition(Status.ToString(), nameof(ProjectStatus.OnHold)));

        Status = ProjectStatus.OnHold;
        Touch();
        RaiseDomainEvent(new ProjectStatusChangedEvent(Id.Value, TenantId, ProjectStatus.OnHold));
        return Result.Ok();
    }

    public Result Complete()
    {
        if (Status != ProjectStatus.Active)
            return Result.Fail(
                ProjectErrors.InvalidStatusTransition(Status.ToString(), nameof(ProjectStatus.Completed)));

        Status = ProjectStatus.Completed;
        Touch();
        RaiseDomainEvent(new ProjectStatusChangedEvent(Id.Value, TenantId, ProjectStatus.Completed));
        return Result.Ok();
    }

    public Result Cancel()
    {
        if (Status == ProjectStatus.Completed)
            return Result.Fail(
                ProjectErrors.InvalidStatusTransition(Status.ToString(), nameof(ProjectStatus.Cancelled)));

        Status = ProjectStatus.Cancelled;
        Touch();
        RaiseDomainEvent(new ProjectStatusChangedEvent(Id.Value, TenantId, ProjectStatus.Cancelled));
        return Result.Ok();
    }

    // ── Details ───────────────────────────────────────────────
    public Result UpdateDetails(
        string name,
        string? description,
        string? clientName,
        string? location,
        DateTime? startDate,
        DateTime? endDate,
        Guid modifiedByUserId)
    {
        if (string.IsNullOrWhiteSpace(name))
            return Result.Fail(ProjectErrors.NameRequired);

        if (endDate.HasValue && startDate.HasValue && endDate < startDate)
            return Result.Fail(ProjectErrors.EndDateBeforeStart);

        Name        = name.Trim();
        Description = description?.Trim();
        ClientName  = clientName?.Trim();
        Location    = location?.Trim();
        StartDate   = startDate;
        EndDate     = endDate;
        Touch(modifiedByUserId);
        return Result.Ok();
    }

    public Result UpdateBudget(decimal amount, string currency, Guid modifiedByUserId)
    {
        Budget = Money.Create(amount, currency);
        Touch(modifiedByUserId);
        return Result.Ok();
    }

    // Small helper to keep audit fields consistent.
    private void Touch(Guid? modifiedBy = null)
    {
        ModifiedAtUtc = DateTime.UtcNow;
        if (modifiedBy.HasValue) ModifiedBy = modifiedBy;
    }

    // ── Members (Result pattern) ──────────────────────────────
    public Result AddMember(Guid userId, ProjectMemberRole role)
    {
        if (_members.Any(m => m.UserId == userId))
            return Result.Fail(ProjectErrors.UserAlreadyMember(userId));

        _members.Add(ProjectMember.Create(Id, userId, role));
        RaiseDomainEvent(new ProjectMemberAddedEvent(Id.Value, TenantId, userId, role));
        return Result.Ok();
    }

    public Result RemoveMember(Guid userId)
    {
        var member = _members.FirstOrDefault(m => m.UserId == userId);
        if (member is null)
            return Result.Fail(ProjectErrors.UserNotMember(userId));

        // A project must always keep at least one Lead.
        if (member.Role == ProjectMemberRole.Lead
            && _members.Count(m => m.Role == ProjectMemberRole.Lead) == 1)
            return Result.Fail(ProjectErrors.LastLead);

        _members.Remove(member);
        return Result.Ok();
    }

    public Result ChangeMemberRole(Guid userId, ProjectMemberRole newRole)
    {
        var member = _members.FirstOrDefault(m => m.UserId == userId);
        if (member is null)
            return Result.Fail(ProjectErrors.UserNotMember(userId));

        // Prevent demoting the only Lead, which would leave none.
        if (member.Role == ProjectMemberRole.Lead
            && newRole != ProjectMemberRole.Lead
            && _members.Count(m => m.Role == ProjectMemberRole.Lead) == 1)
            return Result.Fail(ProjectErrors.LastLead);

        member.ChangeRole(newRole);
        Touch();
        return Result.Ok();
    }

    public bool HasMember(Guid userId) => _members.Any(m => m.UserId == userId);
    public bool IsLead(Guid userId) =>
        _members.Any(m => m.UserId == userId && m.Role == ProjectMemberRole.Lead);
}