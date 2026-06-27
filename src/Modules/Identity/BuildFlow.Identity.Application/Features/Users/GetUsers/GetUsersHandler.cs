using BuildFlow.Application.Abstractions;
using BuildFlow.Identity.Application.Abstractions;
using BuildFlow.Identity.Domain.Users;
using FluentResults;

namespace BuildFlow.Identity.Application.Features.Users.GetUsers;

internal sealed class GetUsersHandler(
    ICurrentUserService currentUser,
    IUserRepository userRepository)
    : IQueryHandler<GetUsersQuery, PagedResult<UserDto>>
{
    public async Task<Result<PagedResult<UserDto>>> Handle(
        GetUsersQuery query,
        CancellationToken cancellationToken)
    {
        // المستأجر من السياق — نجلب مستخدميه فقط
        var (users, totalCount) = await userRepository.GetPagedByTenantAsync(
            currentUser.TenantId,
            query.Page,
            query.PageSize,
            cancellationToken);

        // حوّل الكيانات إلى DTOs
        var dtos = users
            .Select(u => new UserDto(
                u.Id.Value,
                u.Email.Value,
                u.FullName,
                u.Role.ToString(),
                u.Status.ToString()))
            .ToList();

        var pagedResult = new PagedResult<UserDto>(
            dtos, totalCount, query.Page, query.PageSize);

        return Result.Ok(pagedResult);
    }
}