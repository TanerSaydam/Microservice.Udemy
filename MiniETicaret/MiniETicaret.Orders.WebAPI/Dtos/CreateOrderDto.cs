namespace MiniETicaret.Orders.WebAPI.Dtos;

public sealed record CreateOrderDto(
    Guid ProductId,
    int Quantity,
    decimal Price);
