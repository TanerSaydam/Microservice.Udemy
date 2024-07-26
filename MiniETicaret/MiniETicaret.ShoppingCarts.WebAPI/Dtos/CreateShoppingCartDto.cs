namespace MiniETicaret.ShoppingCarts.WebAPI.Dtos;

public sealed record CreateShoppingCartDto(
    Guid ProductId,
    int Quantity);

public sealed record ShoppingCartDto
{
    public Guid Id { get; set; }
    public Guid ProductId { get; set; }
    public string ProductName { get; set; } = default!;
    public decimal ProductPrice { get; set; }
    public int Quantity { get; set; }
}
