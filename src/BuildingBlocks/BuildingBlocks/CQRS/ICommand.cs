using MediatR;

namespace BuildingBlocks.CQRS;

public interface ICommand : ICommand<Unit>  //used when we dont require the command to return any result. void like behavior
{

}
public interface ICommand<out TResponse> : IRequest<TResponse> //used when we need to return some response, TResponse is generic response.
{
}

