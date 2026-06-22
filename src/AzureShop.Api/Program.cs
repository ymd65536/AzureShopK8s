var builder = WebApplication.CreateBuilder(args);
builder.AddServiceDefaults();
builder.Services.AddOpenApi();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.MapDefaultEndpoints();

var products = new[]
{
    new Product(1, "Azure Mug", 19.99m, "A ceramic mug with the Azure cloud logo."),
    new Product(2, "AKS Hoodie", 49.50m, "Comfortable hoodie for your Kubernetes team."),
    new Product(3, "Monitor Bundle", 129.00m, "A complete starter bundle for observability.")
};

app.MapGet("/health", () => Results.Ok(new { status = "ok" }));
app.MapGet("/api/products", () => Results.Ok(products));
app.MapPost("/api/orders", (OrderRequest request) =>
{
    var response = new { message = "Order received", itemCount = request.Items.Count };
    return Results.Accepted($"/api/orders/{request.Items.Count}", response);
});

app.Run();

public record Product(int Id, string Name, decimal Price, string Description);
public record OrderRequest(List<int> Items);
