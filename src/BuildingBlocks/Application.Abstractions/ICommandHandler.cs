using FluentResults;
using MediatR;

namespace BuildFlow.Application.Abstractions;

// معالج أمر بلا قيمة إرجاع — يطابق ICommand
public interface ICommandHandler<TCommand> : IRequestHandler<TCommand, Result>
    where TCommand : ICommand
{
}

// معالج أمر يُرجع قيمة — يطابق ICommand<TResponse>
public interface ICommandHandler<TCommand, TResponse>
    : IRequestHandler<TCommand, Result<TResponse>>
    where TCommand : ICommand<TResponse>
{
}