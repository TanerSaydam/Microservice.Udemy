namespace MiniETicaret.ShoppingCarts.WebAPI.Models;

public sealed class ShoppingCart
{
    public ShoppingCart()
    {
        Id = Guid.NewGuid();
    }
    public Guid Id { get; set; }
    public Guid ProductId { get; set; }
    public int Quantity { get; set; }
}
