using MiniETicaret.Orders.WebAPI.Context;
using MiniETicaret.Orders.WebAPI.Dtos;
using MiniETicaret.Orders.WebAPI.Models;
using MiniETicaret.Orders.WebAPI.Options;
using MongoDB.Driver;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<MongoDbSettings>(builder.Configuration.GetSection("MongoDbSettings"));
builder.Services.AddSingleton<MongoDbContext>();

var app = builder.Build();

app.MapGet("/", () => "Hello World!");

app.MapGet("/getall", async (MongoDbContext context, IConfiguration configuration) =>
{
    var items = context.GetCollection<Order>("Orders");

    var orders = await items.Find(item => true).ToListAsync();

    List<OrderDto> orderDtos = new();

    Result<List<ProductDto>>? products = new();

    HttpClient httpClient = new();
    string productsEnpoint = $"http://{configuration.GetSection("HttpRequest:Products").Value}/getall";
    var message = await httpClient.GetAsync(productsEnpoint);

    if (message.IsSuccessStatusCode)
    {
        products = await message.Content.ReadFromJsonAsync<Result<List<ProductDto>>>();
    }

    foreach (var order in orders)
    {
        OrderDto orderDto = new()
        {
            Id = order.Id,
            CreatAt = order.CreatAt,
            ProductId = order.ProductId,
            Quantity = order.Quantity,
            Price = order.Price,
            ProductName = products!.Data!.First(p => p.Id == order.ProductId).Name,
        };

        orderDtos.Add(orderDto);
    }

    return Results.Ok(new Result<List<OrderDto>>(orderDtos));

});

app.MapPost("/create", async (MongoDbContext context, List<CreateOrderDto> request) =>
{
    var items = context.GetCollection<Order>("Orders");
    List<Order> orders = new();
    foreach (var item in request)
    {
        Order order = new()
        {
            ProductId = item.ProductId,
            Quantity = item.Quantity,
            Price = item.Price,
            CreatAt = DateTime.Now,
        };

        orders.Add(order);
    }

    await items.InsertManyAsync(orders);

    return Results.Ok(new Result<string>("Sipariþ baþarýyla oluþturuldu"));
});

app.Run();
