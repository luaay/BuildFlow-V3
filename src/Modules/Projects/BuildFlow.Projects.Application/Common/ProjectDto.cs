namespace BuildFlow.Projects.Application.Common;

public record ProjectDto(
    Guid Id,
    string Name,
    string Code,
    string? Description,
    string Status,
    decimal BudgetAmount,
    string BudgetCurrency,
    string? ClientName,
    string? Location,
    DateTime? StartDate,
    DateTime? EndDate,
    int MemberCount,
    DateTime CreatedAtUtc);

public record ProjectMemberDto(
    Guid UserId,
    string Role,
    DateTime JoinedAtUtc);

public record ProjectDetailDto(
    Guid Id,
    string Name,
    string Code,
    string? Description,
    string Status,
    decimal BudgetAmount,
    string BudgetCurrency,
    string? ClientName,
    string? Location,
    DateTime? StartDate,
    DateTime? EndDate,
    List<ProjectMemberDto> Members,
    DateTime CreatedAtUtc,
    DateTime? ModifiedAtUtc);