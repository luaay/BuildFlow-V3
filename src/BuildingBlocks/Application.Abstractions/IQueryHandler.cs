using FluentResults;
using MediatR;

namespace BuildFlow.Application.Abstractions;

// معالج استعلام — يطابق IQuery<TResponse>
public interface IQueryHandler<TQuery, TResponse>
    : IRequestHandler<TQuery, Result<TResponse>>
    where TQuery : IQuery<TResponse>
{
}