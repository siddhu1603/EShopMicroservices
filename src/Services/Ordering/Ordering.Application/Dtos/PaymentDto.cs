namespace Ordering.Application.Dtos;

public record PaymentDto(string CardName, string Expiration, string Cvv, string CardNumber, int PaymentMethod);
