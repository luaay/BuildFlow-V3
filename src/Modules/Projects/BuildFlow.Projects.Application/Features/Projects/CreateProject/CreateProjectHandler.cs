using BuildFlow.Application.Abstractions;
using BuildFlow.Projects.Application.Abstractions;
using BuildFlow.Projects.Domain.Entities;
using BuildFlow.Projects.Domain.Errors;
using BuildFlow.Projects.Domain.Repositories;
using FluentResults;

namespace BuildFlow.Projects.Application.Features.Projects.CreateProject;

internal sealed class CreateProjectHandler(
    ICurrentUserService currentUser,
    IProjectRepository projectRepository,
    IUnitOfWork unitOfWork)
    : ICommandHandler<CreateProjectCommand, Guid>
{
    public async Task<Result<Guid>> Handle(
        CreateProjectCommand command,
        CancellationToken cancellationToken)
    {
        // Tenant from the token, not the request.
        var tenantId = currentUser.TenantId;

        // Enforce per-tenant code uniqueness before creating.
        if (await projectRepository.CodeExistsAsync(command.Code, tenantId, cancellationToken))
            return Result.Fail(ProjectErrors.CodeAlreadyExists(command.Code));

        // Build the aggregate through its factory (Result pattern).
        var result = Project.Create(
            tenantId,
            command.Name,
            command.Code,
            command.Description,
            command.Budget,
            command.Currency,
            currentUser.UserId,
            command.ClientName,
            command.Location,
            command.StartDate,
            command.EndDate);

        if (result.IsFailed)
            return Result.Fail(result.Errors);

        var project = result.Value;

        await projectRepository.AddAsync(project, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Ok(project.Id.Value);
    }
}