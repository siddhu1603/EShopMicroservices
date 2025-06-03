using Microsoft.AspNetCore.Mvc;

namespace Catalog.API.Products.DeleteProduct;

public record DeleteProductRequest(Guid Id); //Not needed if we use the /{id} version.
public record DeleteProductResponse(bool IsSuccess);
public class DeleteProductEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        //app.MapDelete("/products/{Id}", async (Guid Id, ISender sender) =>
        //{
        //    var result = await sender.Send(new DeleteProductCommand(Id));
        //    var response = result.Adapt<DeleteProductResponse>();

        //    return Results.Ok(response);
        //})
        //    .WithName("DeleteProduct")
        //    .Produces<DeleteProductResponse>(StatusCodes.Status200OK)
        //    .ProducesProblem(StatusCodes.Status400BadRequest)
        //    .ProducesProblem(StatusCodes.Status404NotFound)
        //    .WithSummary("Delete Product")
        //    .WithDescription("Delete product");

        app.MapDelete("/products", async ([FromBody] DeleteProductRequest request, ISender sender) =>  //req body exists so no need of sending a new command obj
        {
            // Here we specifically need to mention [FromBody] because
            //by default ASP.NET core doesnt expect a request body for DELETE rqeuest

            //var command = new DeleteProductCommand(request.Id);
            var command = request.Adapt<DeleteProductCommand>();

            var result = await sender.Send(command);
            var response = result.Adapt<DeleteProductResponse>();

            return Results.Ok(response);
        })
            .WithName("DeleteProduct")
            .Produces<DeleteProductResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithSummary("Delete Product")
            .WithDescription("Delete product");

    }
}

