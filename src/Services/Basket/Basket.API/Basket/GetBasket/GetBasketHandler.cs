namespace Basket.API.Basket.GetBasket
{
    public record GetBasketQuery(string UserName):IQuery<GetBasketResult>; //based on username it will return the Cart, so add it as a property to record.
    public record GetBasketResult(ShoppingCart Cart);                      //the result here shld be a Cart so GetBasketResult has ShoppingCart as a property

    public class GetBasketQueryHandler(IBasketRepository repository) : IQueryHandler<GetBasketQuery, GetBasketResult>
    {
        public async Task<GetBasketResult> Handle(GetBasketQuery query, CancellationToken cancellationToken)
        {
            var basket = await repository.GetBasket(query.UserName);
            return new GetBasketResult(basket);
        }
    }
}
