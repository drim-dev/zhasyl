var builder = DistributedApplication.CreateBuilder(args);

var api = builder.AddProject<Projects.Zhasyl_Api>("api");

builder.AddJavaScriptApp("frontend", "../frontend", "dev")
    .WithReference(api)
    .WithEnvironment("API_BASE_URL", api.GetEndpoint("http"))
    .WithHttpEndpoint(port: 3000, env: "PORT")
    .WithExternalHttpEndpoints()
    .WaitFor(api);

builder.Build().Run();
