using Azure.Core;
using Carter;
using Mapster;
using MediatR;
using Ordering.Application.Orders.Commands.DeleteOrder;

namespace Ordering.API.Endpoints
{
    /// <summary>
    ///  - Takes an OrderId as input
    ///  - constructs a DeleteOrderCommand
    ///  - Sends this to an appropriate CommandHandler - DeleteOrderCommandHandler
    ///  - Returns successfully deleted or notfound.
    /// </summary>
    /// 
    //public record DeleteOrderRequest(Guid OrderId);  //not needeed as we will be sending the orderId directly in the request URL.
    public record DeleteOrderResponse(bool IsSuccess);
    public class DeleteOrder : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapDelete("/orders/{orderId}", async(Guid orderId, ISender sender) =>
            {
                var result = await sender.Send(new DeleteOrderCommand(orderId));  //since we dont have a DeleteOrderRequest and are not mapping it to a command
                                                                                  //we need to send a new DeleteOrderCommand obj with the Id of the Order.
                var response = result.Adapt<DeleteOrderResponse>();
                return Results.Ok(response);
            })
                .WithName("DeleteOrder")
                .Produces<DeleteOrderResponse>(StatusCodes.Status200OK)
                .ProducesProblem(StatusCodes.Status400BadRequest)
                .ProducesProblem(StatusCodes.Status404NotFound)
                .WithSummary("Delete Order")
                .WithDescription("Delete Order");
        }
    }
}
