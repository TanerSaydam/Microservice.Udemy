using Bogus;
using Microsoft.EntityFrameworkCore;
using MiniETicaret.Products.WebAPI.Context;
using MiniETicaret.Products.WebAPI.Dtos;
using MiniETicaret.Products.WebAPI.Models;
using TS.Result;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("SqlServer"));
});

var app = builder.Build();

app.MapGet("/", () => "Hello World!");

app.MapGet("/seedData", (ApplicationDbContext context) =>
{
    for (int i = 0; i < 100; i++)
    {
        Faker faker = new();

        Product product = new()
        {
            Name = faker.Commerce.ProductName(),
            Price = Convert.ToDecimal(faker.Commerce.Price()),
            Stock = faker.Commerce.Random.Int(1, 100)
        };

        context.Products.Add(product);
    }

    context.SaveChanges();

    return Results.Ok(Result<string>.Succeed("Seed data baþarýyla çalýþtýrýldý ve ürünler oluþturuldu"));
});

app.MapGet("/getall", async (ApplicationDbContext context, CancellationToken cancellationToken) =>
{
    var producst = await context.Products.OrderBy(p => p.Name).ToListAsync(cancellationToken);
    Result<List<Product>> response = producst;
    return response;
});

app.MapPost("/create", async (CreateProductDto request, ApplicationDbContext context, CancellationToken cancellationToken) =>
{
    bool isNameExists = await context.Products.AnyAsync(p => p.Name == request.Name, cancellationToken);

    if (isNameExists)
    {
        var response = Result<string>.Failure("Ürün adý daha önce oluþturulmuþ");
        return Results.BadRequest(response);
    }

    Product product = new()
    {
        Name = request.Name,
        Price = request.Price,
        Stock = request.Stock,
    };

    await context.AddAsync(product, cancellationToken);
    await context.SaveChangesAsync(cancellationToken);

    return Results.Ok(Result<string>.Succeed("Ürün kaydý baþarýyla oluþturuldu"));
});

app.MapPost("/change-product-stock", async (List<ChangeProductStockDto> request, ApplicationDbContext context, CancellationToken cancellationToken) =>
{
    foreach (var item in request)
    {
        Product? product = await context.Products.FindAsync(item.ProductId, cancellationToken);
        if (product is not null)
        {
            product.Stock -= item.Quantity;
        }
    }

    await context.SaveChangesAsync(cancellationToken);

    return Results.NoContent();
});

using (var scoped = app.Services.CreateScope())
{
    var srv = scoped.ServiceProvider;
    var context = srv.GetRequiredService<ApplicationDbContext>();
    context.Database.Migrate();
}

app.Run();
