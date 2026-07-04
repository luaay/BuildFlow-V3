using BuildFlow.Projects.Domain.Entities;
using BuildFlow.Projects.Domain.Enums;
using BuildFlow.Projects.Domain.Events;
using FluentAssertions;
using Xunit;

namespace BuildFlow.Projects.Domain.UnitTests.Entities;

public class ProjectTests
{
    // helper لإنشاء مشروع اختباريّ صالح
    private static Project CreateTestProject()
    {
        var result = Project.Create(
            tenantId: Guid.NewGuid(),
            name: "Test Project",
            code: "PRJ-001",
            description: "A test project",
            budget: 1000m,
            currency: "USD",
            createdByUserId: Guid.NewGuid());

        return result.Value;
    }

    // ── Factory ───────────────────────────────────────────────
    [Fact]
    public void Create_WithValidData_ShouldSucceedAndStartInPlanning()
    {
        // Act
        var result = Project.Create(
            tenantId: Guid.NewGuid(),
            name: "Test Project",
            code: "PRJ-001",
            description: null,
            budget: 500m,
            currency: "USD",
            createdByUserId: Guid.NewGuid());

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Status.Should().Be(ProjectStatus.Planning);
        result.Value.Name.Should().Be("Test Project");
    }

    [Fact]
    public void Create_ShouldMakeCreatorALead()
    {
        // Arrange
        var creatorId = Guid.NewGuid();

        // Act
        var result = Project.Create(
            tenantId: Guid.NewGuid(),
            name: "Test Project",
            code: "PRJ-002",
            description: null,
            budget: 500m,
            currency: "USD",
            createdByUserId: creatorId);

        // Assert
        result.Value.IsLead(creatorId).Should().BeTrue();
        result.Value.Members.Should().ContainSingle();
    }

    [Fact]
    public void Create_ShouldRaiseProjectCreatedEvent()
    {
        // Act
        var project = CreateTestProject();

        // Assert
        project.DomainEvents.Should().ContainSingle()
            .Which.Should().BeOfType<ProjectCreatedEvent>();
    }

    [Fact]
    public void Create_WithEmptyName_ShouldFail()
    {
        // Act
        var result = Project.Create(
            tenantId: Guid.NewGuid(),
            name: "   ",
            code: "PRJ-003",
            description: null,
            budget: 500m,
            currency: "USD",
            createdByUserId: Guid.NewGuid());

        // Assert
        result.IsFailed.Should().BeTrue();
    }

    [Fact]
    public void Create_WithEndDateBeforeStart_ShouldFail()
    {
        // Act
        var result = Project.Create(
            tenantId: Guid.NewGuid(),
            name: "Test Project",
            code: "PRJ-004",
            description: null,
            budget: 500m,
            currency: "USD",
            createdByUserId: Guid.NewGuid(),
            startDate: new DateTime(2026, 1, 10),
            endDate: new DateTime(2026, 1, 1));

        // Assert
        result.IsFailed.Should().BeTrue();
    }

    // ── Lifecycle ─────────────────────────────────────────────
    [Fact]
    public void Activate_FromPlanning_ShouldSucceed()
    {
        // Arrange
        var project = CreateTestProject();

        // Act
        var result = project.Activate();

        // Assert
        result.IsSuccess.Should().BeTrue();
        project.Status.Should().Be(ProjectStatus.Active);
    }

    [Fact]
    public void Complete_FromActive_ShouldSucceed()
    {
        // Arrange
        var project = CreateTestProject();
        project.Activate();

        // Act
        var result = project.Complete();

        // Assert
        result.IsSuccess.Should().BeTrue();
        project.Status.Should().Be(ProjectStatus.Completed);
    }

    [Fact]
    public void Complete_FromPlanning_ShouldFail()
    {
        // Arrange
        var project = CreateTestProject();

        // Act — لا يمكن إكمال مشروع في التخطيط مباشرةً
        var result = project.Complete();

        // Assert
        result.IsFailed.Should().BeTrue();
        project.Status.Should().Be(ProjectStatus.Planning);
    }

    [Fact]
    public void PutOnHold_FromActive_ShouldSucceed()
    {
        // Arrange
        var project = CreateTestProject();
        project.Activate();

        // Act
        var result = project.PutOnHold();

        // Assert
        result.IsSuccess.Should().BeTrue();
        project.Status.Should().Be(ProjectStatus.OnHold);
    }

    [Fact]
    public void Cancel_CompletedProject_ShouldFail()
    {
        // Arrange
        var project = CreateTestProject();
        project.Activate();
        project.Complete();

        // Act — المشروع المكتمل لا يُلغى
        var result = project.Cancel();

        // Assert
        result.IsFailed.Should().BeTrue();
        project.Status.Should().Be(ProjectStatus.Completed);
    }


    // ── Members ───────────────────────────────────────────────
    [Fact]
    public void AddMember_NewUser_ShouldSucceed()
    {
        // Arrange
        var project = CreateTestProject();
        var userId = Guid.NewGuid();

        // Act
        var result = project.AddMember(userId, ProjectMemberRole.Engineer);

        // Assert
        result.IsSuccess.Should().BeTrue();
        project.HasMember(userId).Should().BeTrue();
    }

    [Fact]
    public void AddMember_DuplicateUser_ShouldFail()
    {
        // Arrange
        var project = CreateTestProject();
        var userId = Guid.NewGuid();
        project.AddMember(userId, ProjectMemberRole.Engineer);

        // Act — إضافة العضو نفسه ثانيةً
        var result = project.AddMember(userId, ProjectMemberRole.Viewer);

        // Assert
        result.IsFailed.Should().BeTrue();
    }

    [Fact]
    public void AddMember_ShouldRaiseMemberAddedEvent()
    {
        // Arrange
        var project = CreateTestProject();
        var userId = Guid.NewGuid();

        // Act
        project.AddMember(userId, ProjectMemberRole.Reviewer);

        // Assert — حدث الإنشاء أولاً، ثم حدث إضافة العضو
        project.DomainEvents.Should().Contain(e => e is ProjectMemberAddedEvent);
    }

    [Fact]
    public void RemoveMember_ExistingNonLead_ShouldSucceed()
    {
        // Arrange
        var project = CreateTestProject();
        var userId = Guid.NewGuid();
        project.AddMember(userId, ProjectMemberRole.Engineer);

        // Act
        var result = project.RemoveMember(userId);

        // Assert
        result.IsSuccess.Should().BeTrue();
        project.HasMember(userId).Should().BeFalse();
    }

    [Fact]
    public void RemoveMember_NotAMember_ShouldFail()
    {
        // Arrange
        var project = CreateTestProject();

        // Act — إزالة مستخدم ليس عضواً
        var result = project.RemoveMember(Guid.NewGuid());

        // Assert
        result.IsFailed.Should().BeTrue();
    }

    [Fact]
    public void RemoveMember_LastLead_ShouldFail()
    {
        // Arrange — المنشئ هو القائد الوحيد
        var creatorId = Guid.NewGuid();
        var result = Project.Create(
            tenantId: Guid.NewGuid(),
            name: "Test Project",
            code: "PRJ-100",
            description: null,
            budget: 500m,
            currency: "USD",
            createdByUserId: creatorId);
        var project = result.Value;

        // Act — محاولة إزالة القائد الوحيد
        var removeResult = project.RemoveMember(creatorId);

        // Assert — يجب أن يبقى قائد واحد على الأقلّ
        removeResult.IsFailed.Should().BeTrue();
        project.IsLead(creatorId).Should().BeTrue();
    }

    [Fact]
    public void ChangeMemberRole_ExistingMember_ShouldSucceed()
    {
        // Arrange
        var project = CreateTestProject();
        var userId = Guid.NewGuid();
        project.AddMember(userId, ProjectMemberRole.Viewer);

        // Act
        var result = project.ChangeMemberRole(userId, ProjectMemberRole.Engineer);

        // Assert
        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public void ChangeMemberRole_DemotingLastLead_ShouldFail()
    {
        // Arrange — المنشئ هو القائد الوحيد
        var creatorId = Guid.NewGuid();
        var result = Project.Create(
            tenantId: Guid.NewGuid(),
            name: "Test Project",
            code: "PRJ-101",
            description: null,
            budget: 500m,
            currency: "USD",
            createdByUserId: creatorId);
        var project = result.Value;

        // Act — محاولة تنزيل القائد الوحيد إلى دور آخر
        var changeResult = project.ChangeMemberRole(creatorId, ProjectMemberRole.Viewer);

        // Assert — يُمنع، فالمشروع يجب أن يبقى بقائد
        changeResult.IsFailed.Should().BeTrue();
        project.IsLead(creatorId).Should().BeTrue();
    }

}