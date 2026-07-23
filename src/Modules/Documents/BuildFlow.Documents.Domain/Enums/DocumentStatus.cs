namespace BuildFlow.Documents.Domain.Enums;

// حالات سير عمل المراجعة، مرقّمة صراحةً لثبات المخزّن
public enum DocumentStatus
{
    Draft = 1,          // مسوّدة، قابلة للتعديل
    UnderReview = 2,    // قيد المراجعة، مقفلة عن التعديل
    Approved = 3,       // معتمَدة، حالة نهائية
    Rejected = 4,       // مرفوضة، تعود مسوّدةً للتصحيح
    Archived = 5        // مؤرشَفة، حالة نهائية
}