using FluentResults;

namespace BuildFlow.SharedKernel.Domain;

// خطأ تطبيق منظّم — يحمل كوداً ثابتاً إضافة للرسالة
// يوسّع Error من FluentResults
public sealed class AppError : Error
{
    public string Code { get; }

    public AppError(string code, string message) : base(message)
    {
        Code = code;
        // نخزّن الكود في metadata أيضاً ليكون متاحاً للمستهلكين
        Metadata.Add("Code", code);
    }
}