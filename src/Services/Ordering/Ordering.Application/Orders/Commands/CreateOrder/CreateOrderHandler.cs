using BuildingBlocks.CQRS;
using Ordering.Application.Data;
using Ordering.Application.Dtos;
using Ordering.Domain.Models;
using Ordering.Domain.ValueObjects;

namespace Ordering.Application.Orders.Commands.CreateOrder
{
    public class CreateOrderHandler(IApplicationDbContext dbContext) : ICommandHandler<CreateOrderCommand, CreateOrderResult>
    {
        public async Task<CreateOrderResult> Handle(CreateOrderCommand command, CancellationToken cancellationToken)
        {
            var order = CreateNewOrder(command.Order);
            Console.WriteLine($"[DEBUG] Order to save: {order.Id}, Items: {order.OrderItems.Count}");
            dbContext.Orders.Add(order);
            try
            {
                await dbContext.SaveChangesAsync(cancellationToken);
                Console.WriteLine("[DEBUG] Order saved successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Exception during SaveChangesAsync: {ex}");
                throw;
            }
            return new CreateOrderResult(order.Id.Value);
        }

        private Order CreateNewOrder(OrderDto orderDto)  //This builds the actual Order obj from the OrderDto obj.
        {
            var shippingAddress = Address.Of(orderDto.ShippingAddress.Firstname, orderDto.ShippingAddress.LastName, orderDto.ShippingAddress.EmailAddress, orderDto.ShippingAddress.AddressLine, orderDto.ShippingAddress.Country, orderDto.ShippingAddress.State, orderDto.ShippingAddress.ZipCode);
            var billingAddress = Address.Of(orderDto.BillingAddress.Firstname, orderDto.BillingAddress.LastName, orderDto.BillingAddress.EmailAddress, orderDto.BillingAddress.AddressLine, orderDto.BillingAddress.Country, orderDto.BillingAddress.State, orderDto.BillingAddress.ZipCode);
             
            var newOrder = Order.Create(
                    id: OrderId.Of(Guid.NewGuid()),
                    customerId: CustomerId.Of(orderDto.CustomerId),
                    orderName: OrderName.Of(orderDto.OrderName),
                    shippingAddress: shippingAddress,
                    billingAddress: billingAddress,
                    payment: Payment.Of(orderDto.Payment.CardName, orderDto.Payment.CardNumber, orderDto.Payment.Expiration, orderDto.Payment.Cvv, orderDto.Payment.PaymentMethod)
                    );

            foreach (var orderItemDto in orderDto.OrderItems)
            {
                newOrder.Add(ProductId.Of(orderItemDto.ProductId), orderItemDto.Quantity, orderItemDto.Price);
            }

            return newOrder;
        }
    }
}
