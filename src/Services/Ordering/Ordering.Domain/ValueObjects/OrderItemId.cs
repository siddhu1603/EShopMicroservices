using Ordering.Domain.Exceptions;

namespace Ordering.Domain.ValueObjects;

public record OrderItemId
{
    public Guid Value { get; }
    private OrderItemId(Guid value) => Value = value;

    public static OrderItemId Of(Guid value)
    {
        ArgumentNullException.ThrowIfNull(value);
        if (value == Guid.Empty)
        {
            throw new DomainExceptions("OrderItemId cannot be Empty");
        }

        return new OrderItemId(value);
    }
}