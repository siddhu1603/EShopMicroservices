using BuildingBlocks.Pagination;
using Carter;
using Mapster;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Ordering.Application.Dtos;
using Ordering.Application.Orders.Queries.GetOrders;

namespace Ordering.API.Endpoints
{
    /// <summary>
    ///  - Takes a pagination parameters as input
    ///  - constructs a GetOrdersQuery
    ///  - Retrieves data in paginated format.
    /// </summary>

    public record GetOrdersRequest(PaginationRequest PaginationRequest);
    public record GetOrdersResponse(PaginatedResult<OrderDto> Orders);

    public class GetOrders : ICarterModule
    {
        //public void AddRoutes(IEndpointRouteBuilder app)
        //{
        //    app.MapGet("orders", async ([AsParameters] GetOrdersRequest request, ISender sender) =>
        //    {
        //        var result = await sender.Send(new GetOrdersQuery(request.PaginationRequest)); //GetOrdersQuery accepts a PaginationRequest obj as per definition
        //        var response = result.Adapt<GetOrdersResponse>();
        //        return Results.Ok(response);
        //    })
        //        .WithName("GetOrders")
        //        .Produces<GetOrdersResponse>(StatusCodes.Status200OK)
        //        .ProducesProblem(StatusCodes.Status400BadRequest)
        //        .ProducesProblem(StatusCodes.Status404NotFound)
        //        .WithSummary("Get Orders")
        //        .WithDescription("Get Orders");
        //}


        //in the below implementation we have not used the GetOrdersRequest, we are directly passing the PaginationRequest obj as a parameter.
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("/orders", async ([AsParameters] PaginationRequest request, ISender sender) =>
            {
                var result = await sender.Send(new GetOrdersQuery(request));

                var response = result.Adapt<GetOrdersResponse>();

                return Results.Ok(response);
            })
            .WithName("GetOrders")
            .Produces<GetOrdersResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithSummary("Get Orders")
            .WithDescription("Get Orders");
        }

    }
}
