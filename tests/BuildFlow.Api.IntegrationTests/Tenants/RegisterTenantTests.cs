using System.Net;
using System.Net.Http.Json;
using BuildFlow.Identity.Infrastructure.Persistence;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace BuildFlow.Api.IntegrationTests.Tenants;

[Collection(nameof(IntegrationTestCollection))]
public class RegisterTenantTests
{
    private readonly IntegrationTestFactory _factory;

    public RegisterTenantTests(IntegrationTestFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task RegisterTenant_WithValidData_ShouldPersistTenantAndOwner()
    {
        // التجهيز: عميل يخاطب التطبيق، وجسم طلب صحيح
        var client = _factory.CreateClient();
        var request = new
        {
            tenantName = "Integration Test Co",
            slug = "integration-test-co",
            plan = 1,
            ownerEmail = "owner@integration.com",
            ownerPassword = "P@ssw0rd123",
            ownerFullName = "Integration Owner"
        };

        // الفعل: إرسال طلب التسجيل فعلياً
        var response = await client.PostAsJsonAsync("/api/tenants/register", request);

        // التوكيد الأوّل: الاستجابة نجحت
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        // التوكيد الثاني: المستأجر حُفظ فعلاً في قاعدة البيانات
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<IdentityDbContext>();

        var tenant = await db.Tenants
            .FirstOrDefaultAsync(t => t.Slug == "integration-test-co");
        tenant.Should().NotBeNull();
        tenant!.Name.Should().Be("Integration Test Co");

        // التوكيد الثالث: المالك حُفظ أيضاً ضمن المستأجر
        var owner = await db.Users
            .FirstOrDefaultAsync(u => u.TenantId == tenant.Id);
        owner.Should().NotBeNull();
    }
}