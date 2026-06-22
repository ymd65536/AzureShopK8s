var builder = DistributedApplication.CreateBuilder(args);

var api = builder.AddProject<Projects.AzureShop_Api>("api")
    .WithExternalHttpEndpoints();

builder.AddProject<Projects.AzureShop_Web>("web")
    .WithExternalHttpEndpoints()
    .WithReference(api);

builder.Build().Run();
