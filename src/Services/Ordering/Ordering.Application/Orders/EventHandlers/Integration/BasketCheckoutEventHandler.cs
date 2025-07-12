using BuildingBlocks.Messaging.Events;
using MassTransit;
using MediatR;
using Microsoft.Extensions.Logging;
using Ordering.Application.Dtos;
using Ordering.Application.Orders.Commands.CreateOrder;

namespace Ordering.Application.Orders.EventHandlers.Integration
{
    public class BasketCheckoutEventHandler(ISender sender, ILogger<BasketCheckoutEventHandler> logger) : IConsumer<BasketCheckoutEvent>
    {
        //Publishing the event in the Basket microservice triggers the Consume method in the Ordering microservice,
        //connecting the two services via the event-driven architecture.
        //In Ordering microservice, the BasketCheckoutEventHandler class implements IConsumer<BasketCheckoutEvent>.
        //MassTransit automatically routes the published event to any consumers that handle this event type.
        public async Task Consume(ConsumeContext<BasketCheckoutEvent> context)  
        {
            //create new order and start order fulfillment process
            logger.LogInformation("Integration event handler: {IntegrationEvent}", context.Message.GetType().Name);

            var command = MapToCreateOrderCommand(context.Message);
            await sender.Send(command);
        }

        private CreateOrderCommand MapToCreateOrderCommand(BasketCheckoutEvent message)
        {
            var addressDto = new AddressDto(message.FirstName, message.LastName, message.EmailAddress, message.AddressLine, message.Country, message.ZipCode, message.State);
            var paymentDto = new PaymentDto(message.CardName, message.Expiration, message.CVV, message.CardNumber, message.PaymentMethod);
            var orderId = Guid.NewGuid();

            var orderDto = new OrderDto
            (
                Id: orderId,
                CustomerId: message.CustomerId,
                OrderName: message.UserName,
                ShippingAddress: addressDto,
                BillingAddress: addressDto,
                Payment: paymentDto,
                Status: Ordering.Domain.Enums.OrderStatus.Pending,
                OrderItems:
                [
                    new OrderItemDto(orderId, new Guid("c67d6323-e8b1-4bdf-9a75-b0d0d2e7e914"), 2, 400),
                    new OrderItemDto(orderId, new Guid("4f136e9f-ff8c-4c1f-9a33-d12f689bdab8"), 4, 650)
                ]
            );

            return new CreateOrderCommand(orderDto);
        }
    }
}
