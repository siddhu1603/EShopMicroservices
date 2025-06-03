using MediatR;

namespace BuildingBlocks.CQRS;

public interface ICommandHandler<in TCommand> : ICommandHandler<TCommand, Unit>  //takes in a command and returns nothing
    where TCommand : ICommand<Unit>
{

}
public interface ICommandHandler<in TCommand, TResponse> : IRequestHandler<TCommand, TResponse>  //takes command and returns a response
    where TCommand : ICommand<TResponse>
    where TResponse : notnull
{

}

