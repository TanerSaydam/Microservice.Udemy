namespace MiniETicaret.Orders.WebAPI.Dtos;

public sealed record OrderDto
{
    public Guid Id { get; set; }
    public Guid ProductId { get; set; }
    public string ProductName { get; set; } = default!;
    public int Quantity { get; set; }
    public decimal Price { get; set; }
    public DateTime CreatAt { get; set; }
}
