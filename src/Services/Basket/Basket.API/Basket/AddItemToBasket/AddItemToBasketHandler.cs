namespace Basket.API.Basket.AddItemToBasket
{
    public record AddItemToBasketCommand(string UserName, ShoppingCartItem Item) : ICommand<AddItemToBasketResult>;
    public record AddItemToBasketResult(string UserName);
    
    public class AddItemToBasketCommandHandler(IBasketRepository repository)
        : ICommandHandler<AddItemToBasketCommand, AddItemToBasketResult>
    {
        public async Task<AddItemToBasketResult> Handle(AddItemToBasketCommand command, CancellationToken cancellationToken)
        {
            var basket = await repository.GetBasket(command.UserName, cancellationToken) ?? new ShoppingCart(command.UserName);

            var existingItem = basket.Items.FirstOrDefault(x => x.ProductId == command.Item.ProductId);

            if (existingItem != null)
                existingItem.Quantity += command.Item.Quantity;

            else
                basket.Items.Add(command.Item);

            var updatedBasket = await repository.StoreBasket(basket, cancellationToken);

            return new AddItemToBasketResult(updatedBasket.UserName);
        }
    }
}
