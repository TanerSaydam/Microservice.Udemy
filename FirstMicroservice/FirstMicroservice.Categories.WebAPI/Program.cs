using FirstMicroservice.Categories.WebAPI.Context;
using FirstMicroservice.Categories.WebAPI.Dtos;
using FirstMicroservice.Categories.WebAPI.Models;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("SqlServer"));
});

var app = builder.Build();

app.MapGet("/categories/getall", async (ApplicationDbContext context, CancellationToken cancellationToken) =>
{
    var categories = await context.Categories.ToListAsync(cancellationToken);

    return categories;
});

app.MapPost("/categories/create", async (CreateCategoryDto request, ApplicationDbContext context, CancellationToken cancellationToken) =>
{
    bool isNameExists = await context.Categories.AnyAsync(p => p.Name == request.Name, cancellationToken);

    if (isNameExists)
    {
        return Results.BadRequest(new { Message = "Category already exists" });
    }

    Category category = new()
    {
        Name = request.Name,
    };

    await context.Categories.AddAsync(category, cancellationToken);
    await context.SaveChangesAsync(cancellationToken);

    return Results.Ok(new { Message = "Category create is successful" });
});

using (var scoped = app.Services.CreateScope())
{
    var srv = scoped.ServiceProvider;
    var context = srv.GetRequiredService<ApplicationDbContext>();
    context.Database.Migrate();
}

app.Run();
