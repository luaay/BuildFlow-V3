using BuildFlow.Identity.Domain.Tenants;
using BuildFlow.Identity.Domain.Tenants.Enums;
using BuildFlow.Identity.Domain.Tenants.Events;
using FluentAssertions;
using Xunit;

namespace BuildFlow.Identity.Domain.UnitTests.Tenants;

public class TenantTests
{
    [Fact]
    public void Create_WithValidData_ShouldCreateActiveTenant()
    {
        // Act
        var tenant = Tenant.Create("Al Rashid Engineering", "al-rashid", TenantPlan.Professional);

        // Assert
        tenant.Name.Should().Be("Al Rashid Engineering");
        tenant.Slug.Should().Be("al-rashid");
        tenant.Status.Should().Be(TenantStatus.Active);
        tenant.Plan.Should().Be(TenantPlan.Professional);
        tenant.Id.Should().NotBe(default(TenantId));
    }

    [Fact]
    public void Create_ShouldRaiseTenantCreatedEvent()
    {
        // Act
        var tenant = Tenant.Create("Al Rashid Engineering", "al-rashid", TenantPlan.Free);

        // Assert
        tenant.DomainEvents.Should().ContainSingle()
            .Which.Should().BeOfType<TenantCreatedEvent>();
    }

    [Fact]
    public void Create_ShouldNormalizeSlug_ToLowercase()
    {
        // Act
        var tenant = Tenant.Create("Test", "  AL-Rashid  ", TenantPlan.Free);

        // Assert
        tenant.Slug.Should().Be("al-rashid");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_WithEmptyName_ShouldThrow(string? name)
    {
        // Act
        var act = () => Tenant.Create(name!, "valid-slug", TenantPlan.Free);

        // Assert
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Suspend_ShouldChangeStatusAndRaiseEvent()
    {
        // Arrange
        var tenant = Tenant.Create("Test", "test", TenantPlan.Free);
        tenant.ClearDomainEvents(); // ننظّف حدث الإنشاء للتركيز على Suspend

        // Act
        tenant.Suspend();

        // Assert
        tenant.Status.Should().Be(TenantStatus.Suspended);
        tenant.SuspendedAtUtc.Should().NotBeNull();
        tenant.DomainEvents.Should().ContainSingle()
            .Which.Should().BeOfType<TenantSuspendedEvent>();
    }

    [Fact]
    public void Suspend_WhenAlreadySuspended_ShouldBeIdempotent()
    {
        // Arrange
        var tenant = Tenant.Create("Test", "test", TenantPlan.Free);
        tenant.Suspend();
        tenant.ClearDomainEvents();

        // Act — استدعاء ثانٍ
        tenant.Suspend();

        // Assert — لا يطلق حدثاً جديداً ولا يتغيّر شيء
        tenant.Status.Should().Be(TenantStatus.Suspended);
        tenant.DomainEvents.Should().BeEmpty();
    }

    [Fact]
    public void Activate_ShouldRestoreActiveStatus()
    {
        // Arrange
        var tenant = Tenant.Create("Test", "test", TenantPlan.Free);
        tenant.Suspend();

        // Act
        tenant.Activate();

        // Assert
        tenant.Status.Should().Be(TenantStatus.Active);
        tenant.SuspendedAtUtc.Should().BeNull();
    }
}