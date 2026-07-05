using BuildFlow.Application.Abstractions;
using BuildFlow.Projects.Application.Common;

namespace BuildFlow.Projects.Application.Features.Projects.GetProject;

public sealed record GetProjectQuery(Guid ProjectId) : IQuery<ProjectDetailDto>;