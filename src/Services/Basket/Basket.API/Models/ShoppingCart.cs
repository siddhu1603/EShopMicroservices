namespace Basket.API.Models
{
    public class ShoppingCart
    {
        public string UserName { get; set; } = default!; //default! means that assign the default values and trust that it wont be null.
        public List<ShoppingCartItem> Items { get; set; } = new();  //new() to avoid the null referrence exception. no null checks needed if this done.
        public decimal TotalPrice => Items.Sum(x => x.Price * x.Quantity);

        public ShoppingCart(string userName)
        {
            UserName = userName;
        }
        //required for mapping
        public ShoppingCart()
        {
            
        }
    }
}
