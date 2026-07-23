namespace BuildFlow.Documents.Domain.Enums;

// تصنيف المستندات الهندسية
public enum DocumentType
{
    Drawing = 1,        // مخطّط
    Specification = 2,  // مواصفة
    Report = 3,         // تقرير
    Contract = 4,       // عقد
    Permit = 5,         // إجازة
    Other = 6           // غير ذلك
}