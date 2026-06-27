using BuildFlow.Application.Abstractions;

namespace BuildFlow.Identity.Application.Features.Users.GetUsers;

// استعلام جلب مستخدمي المستأجر الحالي (مع pagination)
public sealed record GetUsersQuery(
    int Page = 1,
    int PageSize = 20) : IQuery<PagedResult<UserDto>>;