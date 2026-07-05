using BuildFlow.Application.Abstractions;
using BuildFlow.Projects.Application.Abstractions;
using BuildFlow.Projects.Domain.Entities;
using BuildFlow.Projects.Domain.Errors;
using BuildFlow.Projects.Domain.Repositories;
using FluentResults;

namespace BuildFlow.Projects.Application.Features.Projects.UpdateProject;

internal sealed class UpdateProjectHandler(
    ICurrentUserService currentUser,
    IProjectRepository projectRepository,
    IUnitOfWork unitOfWork)
    : ICommandHandler<UpdateProjectCommand>
{
    public async Task<Result> Handle(
        UpdateProjectCommand command,
        CancellationToken cancellationToken)
    {
        var projectId = new ProjectId(command.ProjectId);

        var project = await projectRepository.GetByIdAsync(
            projectId, currentUser.TenantId, cancellationToken);

        if (project is null)
            return Result.Fail(ProjectErrors.NotFound(projectId));

        // Only a Lead may edit the project.
        if (!project.IsLead(currentUser.UserId))
            return Result.Fail(ProjectErrors.Forbidden);

        // Update details, then budget. Both return a Result.
        var detailsResult = project.UpdateDetails(
            command.Name,
            command.Description,
            command.ClientName,
            command.Location,
            command.StartDate,
            command.EndDate,
            currentUser.UserId);

        if (detailsResult.IsFailed)
            return detailsResult;

        var budgetResult = project.UpdateBudget(
            command.Budget, command.Currency, currentUser.UserId);

        if (budgetResult.IsFailed)
            return budgetResult;

        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Ok();
    }
}