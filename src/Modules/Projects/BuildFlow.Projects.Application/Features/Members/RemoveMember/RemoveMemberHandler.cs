using BuildFlow.Application.Abstractions;
using BuildFlow.Projects.Application.Abstractions;
using BuildFlow.Projects.Domain.Entities;
using BuildFlow.Projects.Domain.Errors;
using BuildFlow.Projects.Domain.Repositories;
using FluentResults;

namespace BuildFlow.Projects.Application.Features.Members.RemoveMember;

internal sealed class RemoveMemberHandler(
    ICurrentUserService currentUser,
    IProjectRepository projectRepository,
    IUnitOfWork unitOfWork)
    : ICommandHandler<RemoveMemberCommand>
{
    public async Task<Result> Handle(
        RemoveMemberCommand command,
        CancellationToken cancellationToken)
    {
        var projectId = new ProjectId(command.ProjectId);

        var project = await projectRepository.GetByIdAsync(
            projectId, currentUser.TenantId, cancellationToken);

        if (project is null)
            return Result.Fail(ProjectErrors.NotFound(projectId));

        // Only a Lead may manage members.
        if (!project.IsLead(currentUser.UserId))
            return Result.Fail(ProjectErrors.Forbidden);

        // Domain guards the last-Lead rule and returns a Result.
        var removeResult = project.RemoveMember(command.UserId);
        if (removeResult.IsFailed)
            return removeResult;

        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Ok();
    }
}