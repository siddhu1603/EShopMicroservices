using Microsoft.AspNetCore.Mvc;

namespace Catalog.API.Products.UpdateProduct;

public record UpdateProductRequest(Guid Id, string Name, string Description, List<string> Category, string ImageFile, decimal Price);
public record UpdateProductResponse(bool isSuccess);

public class UpdateProductEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        //the endpoint doesnt contain a /{id}
        //because we are passing the id in the request body itself
        app.MapPut("/products", async (UpdateProductRequest request, ISender sender) =>
        {
            var command = request.Adapt<UpdateProductCommand>(); //Here we have directly used adapt & not used the new keyword
                                                                 //because we already have a request obj to map the command to
                                                                 //Mapster automatically creates a command obj and map for us
            var result = await sender.Send(command);
            var response = result.Adapt<UpdateProductResponse>();

            return Results.Ok(response);
        })
            .WithName("UpdateProduct")
            .Produces<UpdateProductResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithSummary("Update Product")
            .WithDescription("Update product");


        //app.MapPut("/products/{id}", async (Guid id, UpdateProductRequest request, ISender sender) =>
        //{
        //    var command = new UpdateProductCommand(id, request.Name, request.Description, request.Category, request.ImageFile, request.Price);
        //    var result = await sender.Send(command);
        //    var response = result.Adapt<UpdateProductResponse>();

        //    return Results.Ok(response);
        //})
        //    .WithName("UpdateProduct")
        //    .Produces<UpdateProductResponse>(StatusCodes.Status200OK)
        //    .ProducesProblem(StatusCodes.Status400BadRequest)
        //    .ProducesProblem(StatusCodes.Status404NotFound)
        //    .WithSummary("Update Product")
        //    .WithDescription("Update product");


    }
}

