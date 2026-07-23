using BuildFlow.SharedKernel.Domain;

namespace BuildFlow.Documents.Domain.Entities;

public sealed class DocumentVersion : Entity<DocumentVersionId>
{
    // معرّف المستند قويّ، مرجع داخليّ بين كياني الوحدة
    public DocumentId DocumentId { get; private set; }

    // رقم الإصدار، يتصاعد: 1، 2، 3
    public int VersionNumber { get; private set; }

    // بيانات الملفّ، بلا رفع فعليّ
    public string FileName { get; private set; } = null!;
    public string FilePath { get; private set; } = null!;
    public long FileSizeBytes { get; private set; }
    public string ContentType { get; private set; } = null!;

    public string? ChangeNotes { get; private set; }

    // مرجع المستخدم خام، عابر لحدّ الهوية
    public Guid UploadedBy { get; private set; }
    public DateTime UploadedAtUtc { get; private set; }

    private DocumentVersion() : base() { }

    private DocumentVersion(DocumentVersionId id) : base(id) { }

    // داخليّ: لا يُنشأ الإصدار إلا عبر المستند الذي يملكه
    internal static DocumentVersion Create(
        DocumentId documentId,
        int versionNumber,
        string fileName,
        string filePath,
        long fileSizeBytes,
        string contentType,
        string? changeNotes,
        Guid uploadedBy) =>
        new(DocumentVersionId.New())
        {
            DocumentId = documentId,
            VersionNumber = versionNumber,
            FileName = fileName.Trim(),
            FilePath = filePath.Trim(),
            FileSizeBytes = fileSizeBytes,
            ContentType = contentType.Trim(),
            ChangeNotes = changeNotes,
            UploadedBy = uploadedBy,
            UploadedAtUtc = DateTime.UtcNow
        };
}