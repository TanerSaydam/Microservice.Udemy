namespace MiniETicaret.ShoppingCarts.WebAPI.Dtos;
public sealed record ChangeProductStockDto(
    Guid ProductId,
    int Quantity
    );
