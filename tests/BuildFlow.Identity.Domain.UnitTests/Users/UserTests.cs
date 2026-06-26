using BuildFlow.Identity.Domain.Tenants;
using BuildFlow.Identity.Domain.Users;
using BuildFlow.Identity.Domain.Users.Enums;
using BuildFlow.Identity.Domain.Users.Events;
using FluentAssertions;
using Xunit;

namespace BuildFlow.Identity.Domain.UnitTests.Users;

public class UserTests
{
    // helper لإنشاء مستخدم اختباري
    private static User CreateTestUser()
    {
        var email = Email.Create("user@example.com").Value;
        return User.Create(
            TenantId.New(),
            email,
            "hashed-password",
            "Test User",
            UserRole.Member);
    }

    [Fact]
    public void Create_WithValidData_ShouldCreateActiveUser()
    {
        // Act
        var user = CreateTestUser();

        // Assert
        user.Status.Should().Be(UserStatus.Active);
        user.Role.Should().Be(UserRole.Member);
        user.AccessFailedCount.Should().Be(0);
        user.FullName.Should().Be("Test User");
    }

    [Fact]
    public void Create_ShouldRaiseUserCreatedEvent()
    {
        // Act
        var user = CreateTestUser();

        // Assert
        user.DomainEvents.Should().ContainSingle()
            .Which.Should().BeOfType<UserCreatedEvent>();
    }

    [Fact]
    public void RecordFailedLogin_ShouldIncrementCount()
    {
        // Arrange
        var user = CreateTestUser();

        // Act
        user.RecordFailedLogin();

        // Assert
        user.AccessFailedCount.Should().Be(1);
        user.Status.Should().Be(UserStatus.Active); // لم يُقفل بعد
    }

    [Fact]
    public void RecordFailedLogin_AfterFiveAttempts_ShouldLockAccount()
    {
        // Arrange
        var user = CreateTestUser();

        // Act — 5 محاولات فاشلة
        for (int i = 0; i < 5; i++)
            user.RecordFailedLogin();

        // Assert
        user.Status.Should().Be(UserStatus.Locked);
        user.LockoutEndUtc.Should().NotBeNull();
        user.IsLockedOut().Should().BeTrue();
    }

    [Fact]
    public void RecordSuccessfulLogin_ShouldResetFailedCount()
    {
        // Arrange
        var user = CreateTestUser();
        user.RecordFailedLogin();
        user.RecordFailedLogin();

        // Act
        user.RecordSuccessfulLogin();

        // Assert
        user.AccessFailedCount.Should().Be(0);
        user.LockoutEndUtc.Should().BeNull();
    }

    [Fact]
    public void RecordSuccessfulLogin_WhenLocked_ShouldUnlock()
    {
        // Arrange
        var user = CreateTestUser();
        for (int i = 0; i < 5; i++)
            user.RecordFailedLogin();

        // Act
        user.RecordSuccessfulLogin();

        // Assert
        user.Status.Should().Be(UserStatus.Active);
    }

    [Fact]
    public void ChangeRole_ShouldUpdateRole()
    {
        // Arrange
        var user = CreateTestUser();

        // Act
        user.ChangeRole(UserRole.Admin);

        // Assert
        user.Role.Should().Be(UserRole.Admin);
    }

    [Fact]
    public void Deactivate_ShouldSetInactiveStatus()
    {
        // Arrange
        var user = CreateTestUser();

        // Act
        user.Deactivate();

        // Assert
        user.Status.Should().Be(UserStatus.Inactive);
    }
}