namespace BuildFlow.Identity.Domain.Users.Enums;

public enum UserRole
{
    Owner = 1,    // مالك الـ tenant — كل الصلاحيات
    Admin = 2,    // مدير — كل شيء ما عدا الفواتير
    Manager = 3,  // مدير مشاريع ووثائق
    Member = 4,   // عضو — قراءة وكتابة على مشاريعه
    Viewer = 5    // مشاهد — قراءة فقط
}