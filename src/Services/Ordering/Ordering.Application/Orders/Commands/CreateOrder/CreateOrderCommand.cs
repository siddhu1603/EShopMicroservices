using BuildingBlocks.CQRS;
using FluentValidation;
using FluentValidation.Validators;
using Ordering.Application.Dtos;
using System.Windows.Input;

namespace Ordering.Application.Orders.Commands.CreateOrder;

public record CreateOrderCommand(OrderDto Order) : ICommand<CreateOrderResult>;
public record CreateOrderResult(Guid Id);

public class CreateOrderCommandValidator : AbstractValidator<CreateOrderCommand>
{
    public CreateOrderCommandValidator()
    {
        //x is the CreateOrderCommand Record, x.Order access the OrderDto, x.Order.<prop> access the properties defined under OrderDto
        RuleFor(x => x.Order.OrderName).NotEmpty().WithMessage("OrderItems should not be empty!");  
        RuleFor(x => x.Order.CustomerId).NotNull().WithMessage("Customer Id is required!");
        RuleFor(x => x.Order.OrderItems).NotEmpty().WithMessage("OrderItems should not be empty!");
    }
}
