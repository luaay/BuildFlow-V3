using System.Diagnostics;
using BuildFlow.Application.Abstractions;
using BuildFlow.Application.Abstractions.Caching;
using BuildFlow.Identity.Application.Abstractions;
using BuildFlow.Identity.Domain.Users;
using FluentResults;

namespace BuildFlow.Identity.Application.Features.Users.GetUsers;

internal sealed class GetUsersHandler(
    ICurrentUserService currentUser,
    IUserRepository userRepository,
    ICacheService cache)
    : IQueryHandler<GetUsersQuery, PagedResult<UserDto>>
{
    private static readonly TimeSpan CacheDuration = TimeSpan.FromMinutes(5);

    public async Task<Result<PagedResult<UserDto>>> Handle(
        GetUsersQuery query,
        CancellationToken cancellationToken)
    {
        var tenantId = currentUser.TenantId;
        // var cacheKey = UserCacheKeys.List(tenantId, query.Page, query.PageSize);
        var cacheKey = UserCacheKeys.List(tenantId.Value, query.Page, query.PageSize);

        // اقرأ من المخزن أوّلاً
        var cached = await cache.GetAsync<PagedResult<UserDto>>(
            cacheKey, cancellationToken);

        if (cached is not null)
        {
            Activity.Current?.SetTag("cache.hit", true);
            return Result.Ok(cached);
        }

        Activity.Current?.SetTag("cache.hit", false);

        // المستأجر من السياق — نجلب مستخدميه فقط
        var (users, totalCount) = await userRepository.GetPagedByTenantAsync(
            tenantId,
            query.Page,
            query.PageSize,
            cancellationToken);

        // حوّل الكيانات إلى كائنات النقل
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

        await cache.SetAsync(cacheKey, pagedResult, CacheDuration, cancellationToken);

        return Result.Ok(pagedResult);
    }
}