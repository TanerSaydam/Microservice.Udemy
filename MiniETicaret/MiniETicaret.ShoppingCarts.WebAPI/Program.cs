using Microsoft.EntityFrameworkCore;
using MiniETicaret.ShoppingCarts.WebAPI.Context;
using MiniETicaret.ShoppingCarts.WebAPI.Dtos;
using MiniETicaret.ShoppingCarts.WebAPI.Models;
using System.Text;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("PostgreSql"));
});

var app = builder.Build();

app.MapGet("/", () => "Hello World!");

app.MapGet("/getall", async (ApplicationDbContext context, IConfiguration configuration, CancellationToken cancellationToken) =>
{
    List<ShoppingCart> shoppingCarts = await context.ShoppingCarts.ToListAsync(cancellationToken);
    HttpClient client = new HttpClient();

    string productsEnpoint = $"http://{configuration.GetSection("HttpRequest:Products").Value}/getall";
    var message = await client.GetAsync(productsEnpoint);

    Result<List<ProductDto>>? products = new();

    if (message.IsSuccessStatusCode)
    {
        products = await message.Content.ReadFromJsonAsync<Result<List<ProductDto>>>();
    }

    List<ShoppingCartDto> response = shoppingCarts.Select(s => new ShoppingCartDto()
    {
        Id = s.Id,
        ProductId = s.ProductId,
        Quantity = s.Quantity,
        ProductName = products!.Data!.First(p => p.Id == s.ProductId).Name,
        ProductPrice = products.Data!.First(p => p.Id == s.ProductId).Price
    }).ToList();


    return new Result<List<ShoppingCartDto>>(response);

});

app.MapPost("/create", async (CreateShoppingCartDto request, ApplicationDbContext context, CancellationToken cancellationToken) =>
{
    ShoppingCart shoppingCart = new()
    {
        ProductId = request.ProductId,
        Quantity = request.Quantity
    };

    await context.AddAsync(shoppingCart, cancellationToken);
    await context.SaveChangesAsync(cancellationToken);

    return Results.Ok(new Result<string>("Ürün sepete baþarýyla eklendi"));
});


app.MapGet("/createOrder", async (ApplicationDbContext context, IConfiguration configuration, CancellationToken cancellationToken) =>
{
    List<ShoppingCart> shoppingCarts = await context.ShoppingCarts.ToListAsync(cancellationToken);
    HttpClient client = new HttpClient();

    string productsEnpoint = $"http://{configuration.GetSection("HttpRequest:Products").Value}/getall";
    var message = await client.GetAsync(productsEnpoint);

    Result<List<ProductDto>>? products = new();

    if (message.IsSuccessStatusCode)
    {
        products = await message.Content.ReadFromJsonAsync<Result<List<ProductDto>>>();
    }

    List<CreateOrderDto> response = shoppingCarts.Select(s => new CreateOrderDto
    {
        ProductId = s.ProductId,
        Quantity = s.Quantity,
        Price = products!.Data!.First(p => p.Id == s.ProductId).Price
    }).ToList();

    string ordersEnpoint = $"http://{configuration.GetSection("HttpRequest:Orders").Value}/create";

    string stringJson = JsonSerializer.Serialize(response);
    var content = new StringContent(stringJson, Encoding.UTF8, "application/json");

    var orderMessage = await client.PostAsync(ordersEnpoint, content);

    if (orderMessage.IsSuccessStatusCode)
    {
        List<ChangeProductStockDto> changeProductStockDtos = shoppingCarts.Select(s => new ChangeProductStockDto(s.ProductId, s.Quantity)).ToList();

        productsEnpoint = $"http://{configuration.GetSection("HttpRequest:Products").Value}/change-product-stock";

        string prodctsStringJson = JsonSerializer.Serialize(changeProductStockDtos);
        var productsContent = new StringContent(prodctsStringJson, Encoding.UTF8, "application/json");

        await client.PostAsync(productsEnpoint, productsContent);

        context.RemoveRange(shoppingCarts);
        await context.SaveChangesAsync(cancellationToken);
    }

    return Results.Ok(new Result<string>("Sipariþ baþarýyla oluþturuldu"));
});

using (var scoped = app.Services.CreateScope())
{
    var srv = scoped.ServiceProvider;
    var context = srv.GetRequiredService<ApplicationDbContext>();
    context.Database.Migrate();
}

app.Run();
