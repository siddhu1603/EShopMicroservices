namespace Catalog.API.Products.GetProductByCategory;

//public record GetProductByCategoryRequest();
public record GetProductByCategoryResponse(IEnumerable<Product> Products);
public class GetProductByCategoryEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("products/category/{category}", async (string category, ISender sender) =>
        {
            var result = await sender.Send(new GetProductByCategoryQuery(category));  //send new query obj whenever we dont have req body. :)
            var response = result.Adapt<GetProductByCategoryResponse>();
            return Results.Ok(response);
        })
            .WithName("GetProductByCategory")
            .Produces<GetProductByCategoryResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Get Product By Categoryd")
            .WithDescription("Get Product By Category");
    }
}

