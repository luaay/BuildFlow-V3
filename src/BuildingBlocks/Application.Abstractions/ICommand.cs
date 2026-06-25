using FluentResults;
using MediatR;

// Command = يغيّر شيئاً في النظام
// يرجع Result (نجح/فشل) أو Result<T> (نجح مع بيانات)
// لماذا FluentResults وليس Exception؟
// → Exceptions للأخطاء غير المتوقعة
// → Result للنتائج المتوقعة (validation failed, not found)

namespace BuildFlow.Application.Abstractions;

// أمر بلا قيمة إرجاع محدّدة — يُعيد Result (نجاح/فشل فقط)
public interface ICommand : IRequest<Result>
{
}

// أمر يُعيد قيمة من نوع TResponse عند النجاح — يُعيد Result<TResponse>
public interface ICommand<TResponse> : IRequest<Result<TResponse>>
{
}