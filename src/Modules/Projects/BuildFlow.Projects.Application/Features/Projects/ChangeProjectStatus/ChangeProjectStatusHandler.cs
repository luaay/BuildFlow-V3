using BuildFlow.Application.Abstractions;
using BuildFlow.Projects.Application.Abstractions;
using BuildFlow.Projects.Domain.Entities;
using BuildFlow.Projects.Domain.Enums;
using BuildFlow.Projects.Domain.Errors;
using BuildFlow.Projects.Domain.Repositories;
using FluentResults;

namespace BuildFlow.Projects.Application.Features.Projects.ChangeProjectStatus;

internal sealed class ChangeProjectStatusHandler(
    ICurrentUserService currentUser,
    IProjectRepository projectRepository,
    IUnitOfWork unitOfWork)
    : ICommandHandler<ChangeProjectStatusCommand>
{
    public async Task<Result> Handle(
        ChangeProjectStatusCommand command,
        CancellationToken cancellationToken)
    {
        var projectId = new ProjectId(command.ProjectId);

        var project = await projectRepository.GetByIdAsync(
            projectId, currentUser.TenantId, cancellationToken);

        if (project is null)
            return Result.Fail(ProjectErrors.NotFound(projectId));

        // Each lifecycle method returns a Result; no exceptions to catch.
        var transition = command.TargetStatus switch
        {
            ProjectStatus.Active    => project.Activate(),
            ProjectStatus.OnHold    => project.PutOnHold(),
            ProjectStatus.Completed => project.Complete(),
            ProjectStatus.Cancelled => project.Cancel(),
            _ => Result.Fail(ProjectErrors.InvalidStatusTransition(
                    project.Status.ToString(), command.TargetStatus.ToString()))
        };

        if (transition.IsFailed)
            return transition;

        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Ok();
    }
}