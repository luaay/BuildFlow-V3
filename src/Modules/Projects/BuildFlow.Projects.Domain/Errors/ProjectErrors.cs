using BuildFlow.SharedKernel.Domain;
using BuildFlow.Projects.Domain.Entities;

namespace BuildFlow.Projects.Domain.Errors;

public static class ProjectErrors
{
    public static AppError NotFound(ProjectId id) =>
        new("Project.NotFound", $"Project '{id}' was not found.");

    public static AppError CodeAlreadyExists(string code) =>
        new("Project.CodeAlreadyExists",
            $"Project code '{code}' already exists in this tenant.");

    public static AppError InvalidStatusTransition(string from, string to) =>
        new("Project.InvalidStatusTransition",
            $"Cannot transition project from '{from}' to '{to}'.");

    public static AppError UserAlreadyMember(Guid userId) =>
        new("Project.UserAlreadyMember",
            $"User '{userId}' is already a member of this project.");

    public static AppError UserNotMember(Guid userId) =>
        new("Project.UserNotMember",
            $"User '{userId}' is not a member of this project.");

    public static AppError Forbidden =>
        new("Project.Forbidden",
            "You do not have permission to perform this action on the project.");

    public static AppError NameRequired =>
        new("Project.NameRequired", "Project name is required.");

    public static AppError EndDateBeforeStart =>
        new("Project.EndDateBeforeStart", "End date cannot be before start date.");

    public static AppError LastLead =>
        new("Project.LastLead", "A project must have at least one Lead.");
}