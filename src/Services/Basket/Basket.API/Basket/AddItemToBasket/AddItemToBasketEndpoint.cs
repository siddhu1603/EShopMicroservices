using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace Basket.API.Basket.AddItemToBasket
{
    public record AddItemToBasketRequest(string UserName, ShoppingCartItem Item);
    public record AddItemToBasketResponse(string UserName);
    public class AddItemToBasketEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPut("/basket", async (AddItemToBasketRequest request, ISender sender) =>
            {
                var command = request.Adapt<AddItemToBasketCommand>();
                var result = await sender.Send(command);
                var response = result.Adapt<AddItemToBasketResponse>();

                return Results.Ok(response);
            })
                .WithName("AddItemToBasket")
                .Produces<AddItemToBasketResponse>(StatusCodes.Status201Created)
                .ProducesProblem(StatusCodes.Status400BadRequest)
                .WithSummary("Add Item to Basket")
                .WithDescription("Add an individual item to the shopping basket.");
        }
    }
}
