using Carter;
using Mapster;
using MediatR;
using Ordering.Application.Dtos;
using Ordering.Application.Orders.Queries.GetOrdersByCustomer;

namespace Ordering.API.Endpoints
{
    /// <summary>
    ///  - Takes a customerId parameters as input
    ///  - constructs a GetOrdersByCustomerQuery
    ///  - Retrieves matching data for orders for tha customer.
    /// </summary>
    /// 

    //public record GetOrderByCustomerRequest(Guid CustomerId);  //Not needed as we will be passing the cust name through the req url itself.
    public record GetOrderByCustomerResponse(IEnumerable<OrderDto> Orders);
    public class GetOrdersByCustomer : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("orders/customer/{customerId}", async(Guid customerId, ISender sender) =>
            {
                var result = await sender.Send(new GetOrdersByCustomerQuery(customerId));
                var response = result.Adapt<GetOrderByCustomerResponse>();
                return Results.Ok(response);
            })
                .WithName("GetOrdersByCustomer")
                .Produces<GetOrderByCustomerResponse>(StatusCodes.Status200OK)
                .ProducesProblem(StatusCodes.Status400BadRequest)
                .ProducesProblem(StatusCodes.Status404NotFound)
                .WithSummary("Get Orders By Customer")
                .WithDescription("Get Orders By Customer");
        }
    }
}
