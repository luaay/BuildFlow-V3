using BuildFlow.SharedKernel.Domain;
using BuildFlow.SharedKernel.Domain.Auditing;
using BuildFlow.Documents.Domain.Enums;
using BuildFlow.Documents.Domain.Errors;
using FluentResults;

namespace BuildFlow.Documents.Domain.Entities;

public sealed class Document : AggregateRoot<DocumentId>, IAuditableEntity, ISoftDelete
{
    // مراجع خام على الحدود نحو الوحدات الأخرى
    public Guid TenantId { get; private set; }
    public Guid ProjectId { get; private set; }

    public string Title { get; private set; } = null!;
    public string? Description { get; private set; }
    public DocumentType Type { get; private set; }
    public DocumentStatus Status { get; private set; }

    // المراجع المعيَّن، فارغ حتى تُطلب المراجعة
    public Guid? ReviewerId { get; private set; }
    public DateTime? SubmittedForReviewAtUtc { get; private set; }
    public DateTime? ReviewedAtUtc { get; private set; }
    public string? ReviewNotes { get; private set; }

    private readonly List<DocumentVersion> _versions = [];
    public IReadOnlyList<DocumentVersion> Versions => _versions.AsReadOnly();

    // رقم الإصدار الحاليّ، يتصاعد مع كل إصدار جديد
    public int CurrentVersionNumber { get; private set; }

    // حقول التدقيق والحذف الناعم
   // حقول التدقيق والحذف الناعم، بضوابط عامّة كما تشترط الواجهتان
    public DateTime CreatedAtUtc { get; set; }
    public Guid CreatedBy { get; set; }
    public DateTime? ModifiedAtUtc { get; set; }
    public Guid? ModifiedBy { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAtUtc { get; set; }
    public Guid? DeletedBy { get; set; }

    private Document() : base() { }

    private Document(DocumentId id) : base(id) { }

    // المصنع: ينشئ المستند مسوّدةً بإصداره الأوّل
    public static Result<Document> Create(
        Guid tenantId,
        Guid projectId,
        string title,
        string? description,
        DocumentType type,
        string fileName,
        string filePath,
        long fileSizeBytes,
        string contentType,
        Guid createdBy)
    {
        if (string.IsNullOrWhiteSpace(title))
            return Result.Fail(DocumentErrors.TitleRequired);

        var document = new Document(DocumentId.New())
        {
            TenantId = tenantId,
            ProjectId = projectId,
            Title = title.Trim(),
            Description = description,
            Type = type,
            Status = DocumentStatus.Draft,
            CurrentVersionNumber = 1,
            CreatedAtUtc = DateTime.UtcNow,
            CreatedBy = createdBy
        };

        // كل مستند يولد بإصداره الأوّل
        document._versions.Add(DocumentVersion.Create(
            document.Id, 1, fileName, filePath,
            fileSizeBytes, contentType, null, createdBy));

        return Result.Ok(document);
    }

    // من المسوّدة إلى المراجعة، بتعيين مراجع
    public Result SubmitForReview(Guid reviewerId, Guid submittedBy)
    {
        if (Status != DocumentStatus.Draft)
            return Result.Fail(DocumentErrors.InvalidStatusTransition(
                Status.ToString(), DocumentStatus.UnderReview.ToString()));

        if (reviewerId == Guid.Empty)
            return Result.Fail(DocumentErrors.ReviewerRequired);

        Status = DocumentStatus.UnderReview;
        ReviewerId = reviewerId;
        SubmittedForReviewAtUtc = DateTime.UtcNow;
        ReviewNotes = null;

        Touch(submittedBy);
        return Result.Ok();
    }

    // الاعتماد: المراجع المعيَّن وحده
    public Result Approve(Guid reviewerId, string? notes)
    {
        if (Status != DocumentStatus.UnderReview)
            return Result.Fail(DocumentErrors.InvalidStatusTransition(
                Status.ToString(), DocumentStatus.Approved.ToString()));

        if (ReviewerId != reviewerId)
            return Result.Fail(DocumentErrors.NotTheAssignedReviewer);

        Status = DocumentStatus.Approved;
        ReviewedAtUtc = DateTime.UtcNow;
        ReviewNotes = notes;

        Touch(reviewerId);
        return Result.Ok();
    }

    // الرفض: يعيد المستند مسوّدةً للتصحيح
    public Result Reject(Guid reviewerId, string? notes)
    {
        if (Status != DocumentStatus.UnderReview)
            return Result.Fail(DocumentErrors.InvalidStatusTransition(
                Status.ToString(), DocumentStatus.Rejected.ToString()));

        if (ReviewerId != reviewerId)
            return Result.Fail(DocumentErrors.NotTheAssignedReviewer);

        Status = DocumentStatus.Draft;   // يعود مسوّدةً للتصحيح
        ReviewedAtUtc = DateTime.UtcNow;
        ReviewNotes = notes;
        ReviewerId = null;               // يُحرَّر المراجع

        Touch(reviewerId);
        return Result.Ok();
    }

    // الأرشفة: من المعتمَد فقط، حالة نهائية
    public Result Archive(Guid archivedBy)
    {
        if (Status != DocumentStatus.Approved)
            return Result.Fail(DocumentErrors.InvalidStatusTransition(
                Status.ToString(), DocumentStatus.Archived.ToString()));

        Status = DocumentStatus.Archived;
        Touch(archivedBy);
        return Result.Ok();
    }

    // إضافة إصدار جديد: المسوّدة فقط تقبل إصدارات
    public Result AddVersion(
        string fileName,
        string filePath,
        long fileSizeBytes,
        string contentType,
        string? changeNotes,
        Guid uploadedBy)
    {
        if (Status == DocumentStatus.UnderReview)
            return Result.Fail(DocumentErrors.CannotModifyUnderReview);

        if (Status is DocumentStatus.Approved or DocumentStatus.Archived)
            return Result.Fail(DocumentErrors.CannotModifyFinalized);

        CurrentVersionNumber++;

        _versions.Add(DocumentVersion.Create(
            Id, CurrentVersionNumber, fileName, filePath,
            fileSizeBytes, contentType, changeNotes, uploadedBy));

        Touch(uploadedBy);
        return Result.Ok();
    }

    // تعديل بيانات المستند: محروس بالحالة نفسها
    public Result UpdateDetails(
        string title,
        string? description,
        DocumentType type,
        Guid modifiedBy)
    {
        if (Status == DocumentStatus.UnderReview)
            return Result.Fail(DocumentErrors.CannotModifyUnderReview);

        if (Status is DocumentStatus.Approved or DocumentStatus.Archived)
            return Result.Fail(DocumentErrors.CannotModifyFinalized);

        if (string.IsNullOrWhiteSpace(title))
            return Result.Fail(DocumentErrors.TitleRequired);

        Title = title.Trim();
        Description = description;
        Type = type;

        Touch(modifiedBy);
        return Result.Ok();
    }

    // الإصدار الأحدث، للعرض والتنزيل
    public DocumentVersion? GetLatestVersion() =>
        _versions.OrderByDescending(v => v.VersionNumber).FirstOrDefault();

    // دالّة اللمس: تسجّل من عدّل ومتى
    private void Touch(Guid modifiedBy)
    {
        ModifiedAtUtc = DateTime.UtcNow;
        ModifiedBy = modifiedBy;
    }
}