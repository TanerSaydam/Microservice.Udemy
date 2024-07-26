namespace MiniETicaret.ShoppingCarts.WebAPI.Dtos;

public sealed record ProductDto(
    Guid Id,
    string Name,
     decimal Price,
     int Stock);
