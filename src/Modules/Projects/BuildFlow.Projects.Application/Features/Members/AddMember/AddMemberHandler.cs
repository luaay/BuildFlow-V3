using BuildFlow.Application.Abstractions;
using BuildFlow.Projects.Application.Abstractions;
using BuildFlow.Projects.Domain.Entities;
using BuildFlow.Projects.Domain.Errors;
using BuildFlow.Projects.Domain.Repositories;
using FluentResults;

namespace BuildFlow.Projects.Application.Features.Members.AddMember;

internal sealed class AddMemberHandler(
    ICurrentUserService currentUser,
    IProjectRepository projectRepository,
    IUnitOfWork unitOfWork)
    : ICommandHandler<AddMemberCommand>
{
    public async Task<Result> Handle(
        AddMemberCommand command,
        CancellationToken cancellationToken)
    {
        var projectId = new ProjectId(command.ProjectId);

        var project = await projectRepository.GetByIdAsync(
            projectId, currentUser.TenantId, cancellationToken);

        if (project is null)
            return Result.Fail(ProjectErrors.NotFound(projectId));

        // Only a Lead may manage members, consistent with update.
        if (!project.IsLead(currentUser.UserId))
            return Result.Fail(ProjectErrors.Forbidden);

        // Domain method returns a Result; no exception to catch.
        var addResult = project.AddMember(command.UserId, command.Role);
        if (addResult.IsFailed)
            return addResult;

        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Ok();
    }
}