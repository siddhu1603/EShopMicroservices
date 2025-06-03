using MediatR;
namespace BuildingBlocks.CQRS;

public interface IQueryHandler<in TQuery, TResponse> : IRequestHandler<TQuery, TResponse>  //takes in query, returns a response i.e not null
    where TQuery : IQuery<TResponse>
    where TResponse : notnull
{
}

