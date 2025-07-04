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
                    new OrderItemDto(orderId, new Guid("7bb99494-f1be-47aa-8fba-1fc964bd5942"), 2, 9500),
                    new OrderItemDto(orderId, new Guid("23e1f134-1151-42cf-a9a4-072f8278ec7f"), 4, 20000)
                ]
            );

            return new CreateOrderCommand(orderDto);
        }
    }
}
