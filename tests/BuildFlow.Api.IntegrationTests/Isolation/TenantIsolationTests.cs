using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using FluentAssertions;
using Xunit;

namespace BuildFlow.Api.IntegrationTests.Isolation;

[Collection(nameof(IntegrationTestCollection))]
public class TenantIsolationTests
{
    private readonly IntegrationTestFactory _factory;

    public TenantIsolationTests(IntegrationTestFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task Tenant_CannotSeeAnotherTenantsProjects()
    {
        var client = _factory.CreateClient();

        // 1) أنشئ مستأجرين مختلفين، وخذ رمز كلٍّ
        var tokenA = await RegisterAndLogin(
            client, "tenant-a-iso", "owner@tenant-a.com");
        var tokenB = await RegisterAndLogin(
            client, "tenant-b-iso", "owner@tenant-b.com");

        // 2) بالمستأجر الأوّل، أنشئ مشروعاً
        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", tokenA);
        var createProject = new
        {
            name = "Tenant A Secret Project",
            code = "TA-SECRET-001",
            description = "Only tenant A should see this",
            budget = 10000,
            currency = "USD",
            clientName = "Client A",
            location = "City A",
            startDate = (DateTime?)null,
            endDate = (DateTime?)null
        };
        var createResponse = await client.PostAsJsonAsync(
            "/api/projects", createProject);
        createResponse.StatusCode.Should().Be(HttpStatusCode.Created);

        // 3) بالمستأجر الثاني، اجلب المشاريع
        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", tokenB);
        var listResponse = await client.GetAsync("/api/projects");
        listResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        // 4) التوكيد الحاسم: الثاني لا يرى مشروع الأوّل
        var body = await listResponse.Content.ReadAsStringAsync();
        body.Should().NotContain("TA-SECRET-001",
            because: "tenant B must never see tenant A's project");
        body.Should().NotContain("Tenant A Secret Project");
    }

    // مساعِد: يسجّل مستأجراً ثم يدخل، ويعيد الرمز
    private static async Task<string> RegisterAndLogin(
        HttpClient client, string slug, string email)
    {
        var register = new
        {
            tenantName = slug,
            slug = slug,
            plan = 1,
            ownerEmail = email,
            ownerPassword = "P@ssw0rd123",
            ownerFullName = "Owner"
        };
        await client.PostAsJsonAsync("/api/tenants/register", register);

        var login = new { slug = slug, email = email, password = "P@ssw0rd123" };
        var loginResponse = await client.PostAsJsonAsync("/api/auth/login", login);
        var result = await loginResponse.Content
            .ReadFromJsonAsync<LoginDto>();

        return result!.AccessToken;
    }

    private sealed record LoginDto(string AccessToken);
}