using FluentResults;
using MediatR;

namespace BuildFlow.Application.Abstractions;

// استعلام قراءة — يُرجع دائماً بيانات من نوع TResponse
// لا توجد نسخة "بلا إرجاع" لأن الاستعلام بطبيعته يُرجع بيانات
public interface IQuery<TResponse> : IRequest<Result<TResponse>>
{
}