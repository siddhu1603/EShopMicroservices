namespace Catalog.API.Products.GetProducts;

public record GetProductsRequest(int? PageNumber = 1, int? PageSize = 10);
public record GetProductsResponse(IEnumerable<Product> Products);

public class GetProductsEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/products", async ([AsParameters] GetProductsRequest request, ISender sender) =>
        {
            var query = request.Adapt<GetProductsQuery>();    //added this to implement pagination. takes in pageno and pagesize params
            //var result = await sender.Send(new GetProductsQuery());  //here we are sending a new query so page no and size will always be 1,10
            var result = await sender.Send(query);                     //captures actual values of page no and size as per URL parameters.
            var response = result.Adapt<GetProductsResponse>();
            return Results.Ok(response);
        })
            .WithName("GetProducts")
            .Produces<GetProductsResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Get Products")
            .WithDescription("Get Products");
    }
}

