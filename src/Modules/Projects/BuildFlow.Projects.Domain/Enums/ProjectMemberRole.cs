namespace BuildFlow.Projects.Domain.Enums;

public enum ProjectMemberRole
{
    Lead     = 1,   // Project manager / lead engineer
    Engineer = 2,   // Full read/write
    Reviewer = 3,   // Can review & approve documents
    Viewer   = 4    // Read-only
}