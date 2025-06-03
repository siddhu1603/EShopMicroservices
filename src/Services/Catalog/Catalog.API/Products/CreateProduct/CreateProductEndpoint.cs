namespace Catalog.API.Products.CreateProduct;

public record CreateProductRequest(string Name, string Description, List<string> Category, string ImageFile, decimal Price);
public record CreateProductResponse(Guid Id);
public class CreateProductEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/products", async (CreateProductRequest request, ISender sender) =>
        {
            var command = request.Adapt<CreateProductCommand>();     //mapping the reuest obj to a command obj
            var result = await sender.Send(command);                 //send() method directs the command to appropriate commandhandler
                                                                     //returns a CreateProductResult obj
            var response = result.Adapt<CreateProductResponse>();    //map the result from the comamandhandler back to a response obj
            return Results.Created($"/products/{response.Id}", response);
        })
            .WithName("CreateProduct")
            .Produces<CreateProductResponse>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Create Product")
            .WithDescription("Create product");
    }
}
