using BuildFlow.Documents.Domain.Entities;
using BuildFlow.Documents.Domain.Enums;
using FluentAssertions;
using Xunit;

namespace BuildFlow.Documents.Domain.UnitTests.Entities;

public class DocumentTests
{
    // helper لإنشاء مستند اختباريّ صالح
    private static Document CreateTestDocument(Guid? createdBy = null)
    {
        var result = Document.Create(
            tenantId: Guid.NewGuid(),
            projectId: Guid.NewGuid(),
            title: "Structural Drawing A-101",
            description: "Foundation layout",
            type: DocumentType.Drawing,
            fileName: "A-101.pdf",
            filePath: "/storage/a-101.pdf",
            fileSizeBytes: 245000,
            contentType: "application/pdf",
            createdBy: createdBy ?? Guid.NewGuid());

        return result.Value;
    }

    // ── Factory ───────────────────────────────────────────────
    [Fact]
    public void Create_WithValidData_ShouldStartAsDraft()
    {
        // Act
        var document = CreateTestDocument();

        // Assert
        document.Status.Should().Be(DocumentStatus.Draft);
        document.Title.Should().Be("Structural Drawing A-101");
        document.ReviewerId.Should().BeNull();
    }

    [Fact]
    public void Create_ShouldAddFirstVersion()
    {
        // Act
        var document = CreateTestDocument();

        // Assert — كل مستند يولد بإصداره الأوّل
        document.Versions.Should().ContainSingle();
        document.CurrentVersionNumber.Should().Be(1);
        document.Versions[0].VersionNumber.Should().Be(1);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_WithEmptyTitle_ShouldFail(string title)
    {
        // Act
        var result = Document.Create(
            tenantId: Guid.NewGuid(),
            projectId: Guid.NewGuid(),
            title: title,
            description: null,
            type: DocumentType.Drawing,
            fileName: "f.pdf",
            filePath: "/f.pdf",
            fileSizeBytes: 100,
            contentType: "application/pdf",
            createdBy: Guid.NewGuid());

        // Assert
        result.IsFailed.Should().BeTrue();
    }

    // ── Review workflow ───────────────────────────────────────
    [Fact]
    public void SubmitForReview_FromDraft_ShouldSucceed()
    {
        // Arrange
        var document = CreateTestDocument();
        var reviewerId = Guid.NewGuid();

        // Act
        var result = document.SubmitForReview(reviewerId, Guid.NewGuid());

        // Assert
        result.IsSuccess.Should().BeTrue();
        document.Status.Should().Be(DocumentStatus.UnderReview);
        document.ReviewerId.Should().Be(reviewerId);
        document.SubmittedForReviewAtUtc.Should().NotBeNull();
    }

    [Fact]
    public void SubmitForReview_WithEmptyReviewer_ShouldFail()
    {
        // Arrange
        var document = CreateTestDocument();

        // Act — لا مراجع معيَّن
        var result = document.SubmitForReview(Guid.Empty, Guid.NewGuid());

        // Assert
        result.IsFailed.Should().BeTrue();
        document.Status.Should().Be(DocumentStatus.Draft);
    }

    [Fact]
    public void SubmitForReview_WhenAlreadyUnderReview_ShouldFail()
    {
        // Arrange
        var document = CreateTestDocument();
        document.SubmitForReview(Guid.NewGuid(), Guid.NewGuid());

        // Act — تقديم ثانٍ
        var result = document.SubmitForReview(Guid.NewGuid(), Guid.NewGuid());

        // Assert
        result.IsFailed.Should().BeTrue();
    }

    [Fact]
    public void Approve_FromDraft_ShouldFail()
    {
        // Arrange — مستند لم يُقدَّم للمراجعة
        var document = CreateTestDocument();

        // Act
        var result = document.Approve(Guid.NewGuid(), null);

        // Assert
        result.IsFailed.Should().BeTrue();
        document.Status.Should().Be(DocumentStatus.Draft);
    }

    [Fact]
    public void Archive_FromDraft_ShouldFail()
    {
        // Arrange
        var document = CreateTestDocument();

        // Act — الأرشفة من المعتمَد فقط
        var result = document.Archive(Guid.NewGuid());

        // Assert
        result.IsFailed.Should().BeTrue();
    }

    // ── Reviewer identity ─────────────────────────────────────
    [Fact]
    public void Approve_ByAssignedReviewer_ShouldSucceed()
    {
        // Arrange
        var document = CreateTestDocument();
        var reviewerId = Guid.NewGuid();
        document.SubmitForReview(reviewerId, Guid.NewGuid());

        // Act — المراجع المعيَّن يعتمد
        var result = document.Approve(reviewerId, "Verified.");

        // Assert
        result.IsSuccess.Should().BeTrue();
        document.Status.Should().Be(DocumentStatus.Approved);
        document.ReviewedAtUtc.Should().NotBeNull();
        document.ReviewNotes.Should().Be("Verified.");
    }

    [Fact]
    public void Approve_ByDifferentUser_ShouldFail()
    {
        // Arrange
        var document = CreateTestDocument();
        var assignedReviewer = Guid.NewGuid();
        document.SubmitForReview(assignedReviewer, Guid.NewGuid());

        // Act — مستخدم آخر يحاول الاعتماد
        var result = document.Approve(Guid.NewGuid(), "Sneaky approval.");

        // Assert — يُرفَض، والحالة تصمد
        result.IsFailed.Should().BeTrue();
        document.Status.Should().Be(DocumentStatus.UnderReview);
    }

    [Fact]
    public void Reject_ByDifferentUser_ShouldFail()
    {
        // Arrange
        var document = CreateTestDocument();
        var assignedReviewer = Guid.NewGuid();
        document.SubmitForReview(assignedReviewer, Guid.NewGuid());

        // Act
        var result = document.Reject(Guid.NewGuid(), "Sneaky rejection.");

        // Assert
        result.IsFailed.Should().BeTrue();
        document.Status.Should().Be(DocumentStatus.UnderReview);
    }

    [Fact]
    public void Reject_ByAssignedReviewer_ShouldReturnToDraftAndClearReviewer()
    {
        // Arrange
        var document = CreateTestDocument();
        var reviewerId = Guid.NewGuid();
        document.SubmitForReview(reviewerId, Guid.NewGuid());

        // Act
        var result = document.Reject(reviewerId, "Missing load calculations.");

        // Assert — يعود مسوّدةً، ويُحرَّر المراجع، وتُحفَظ الملاحظات
        result.IsSuccess.Should().BeTrue();
        document.Status.Should().Be(DocumentStatus.Draft);
        document.ReviewerId.Should().BeNull();
        document.ReviewNotes.Should().Be("Missing load calculations.");
    }

    [Fact]
    public void Archive_AfterApproval_ShouldSucceed()
    {
        // Arrange
        var document = CreateTestDocument();
        var reviewerId = Guid.NewGuid();
        document.SubmitForReview(reviewerId, Guid.NewGuid());
        document.Approve(reviewerId, null);

        // Act
        var result = document.Archive(Guid.NewGuid());

        // Assert
        result.IsSuccess.Should().BeTrue();
        document.Status.Should().Be(DocumentStatus.Archived);
    }


    // ── Modification guards ───────────────────────────────────
    [Fact]
    public void AddVersion_WhileDraft_ShouldSucceedAndIncrementNumber()
    {
        // Arrange
        var document = CreateTestDocument();

        // Act
        var result = document.AddVersion(
            "A-101-rev-B.pdf", "/storage/rev-b.pdf", 251000,
            "application/pdf", "Updated dimensions", Guid.NewGuid());

        // Assert — الرقم يتصاعد تلقائياً
        result.IsSuccess.Should().BeTrue();
        document.CurrentVersionNumber.Should().Be(2);
        document.Versions.Should().HaveCount(2);
    }

    [Fact]
    public void AddVersion_WhileUnderReview_ShouldFail()
    {
        // Arrange
        var document = CreateTestDocument();
        document.SubmitForReview(Guid.NewGuid(), Guid.NewGuid());

        // Act — التعديل ممنوع أثناء المراجعة
        var result = document.AddVersion(
            "sneaky.pdf", "/storage/sneaky.pdf", 100,
            "application/pdf", null, Guid.NewGuid());

        // Assert — يُرفَض، ولا يتغيّر شيء
        result.IsFailed.Should().BeTrue();
        document.CurrentVersionNumber.Should().Be(1);
        document.Versions.Should().ContainSingle();
    }

    [Fact]
    public void AddVersion_AfterApproval_ShouldFail()
    {
        // Arrange
        var document = CreateTestDocument();
        var reviewerId = Guid.NewGuid();
        document.SubmitForReview(reviewerId, Guid.NewGuid());
        document.Approve(reviewerId, null);

        // Act — المعتمَد لا يُعدَّل
        var result = document.AddVersion(
            "after.pdf", "/storage/after.pdf", 100,
            "application/pdf", null, Guid.NewGuid());

        // Assert
        result.IsFailed.Should().BeTrue();
        document.Versions.Should().ContainSingle();
    }

    [Fact]
    public void UpdateDetails_WhileUnderReview_ShouldFail()
    {
        // Arrange
        var document = CreateTestDocument();
        document.SubmitForReview(Guid.NewGuid(), Guid.NewGuid());

        // Act — الحراسة نفسها على المسار الآخر
        var result = document.UpdateDetails(
            "New Title", null, DocumentType.Report, Guid.NewGuid());

        // Assert — العنوان لم يتغيّر
        result.IsFailed.Should().BeTrue();
        document.Title.Should().Be("Structural Drawing A-101");
    }

    [Fact]
    public void UpdateDetails_WhileDraft_ShouldSucceed()
    {
        // Arrange
        var document = CreateTestDocument();

        // Act
        var result = document.UpdateDetails(
            "Revised Drawing A-102", "Updated scope",
            DocumentType.Specification, Guid.NewGuid());

        // Assert
        result.IsSuccess.Should().BeTrue();
        document.Title.Should().Be("Revised Drawing A-102");
        document.Type.Should().Be(DocumentType.Specification);
    }

    [Fact]
    public void AddVersion_AfterRejection_ShouldSucceed()
    {
        // Arrange — مستند رُفض فعاد مسوّدةً
        var document = CreateTestDocument();
        var reviewerId = Guid.NewGuid();
        document.SubmitForReview(reviewerId, Guid.NewGuid());
        document.Reject(reviewerId, "Needs revision.");

        // Act — التصحيح ممكن بعد الرفض
        var result = document.AddVersion(
            "A-101-rev-C.pdf", "/storage/rev-c.pdf", 260000,
            "application/pdf", "Added load calculations", Guid.NewGuid());

        // Assert — دورة التصحيح تعمل
        result.IsSuccess.Should().BeTrue();
        document.CurrentVersionNumber.Should().Be(2);
    }

    [Fact]
    public void GetLatestVersion_ShouldReturnHighestVersionNumber()
    {
        // Arrange
        var document = CreateTestDocument();
        document.AddVersion(
            "rev-b.pdf", "/storage/rev-b.pdf", 200,
            "application/pdf", null, Guid.NewGuid());

        // Act
        var latest = document.GetLatestVersion();

        // Assert
        latest.Should().NotBeNull();
        latest!.VersionNumber.Should().Be(2);
        latest.FileName.Should().Be("rev-b.pdf");
    }
}