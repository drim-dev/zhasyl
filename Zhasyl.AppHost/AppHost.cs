var builder = DistributedApplication.CreateBuilder(args);

var postgres = builder.AddPostgres("postgres")
    .WithImageTag("17")
    .WithDataVolume("zhasyl-postgres-data")
    .AddDatabase("zhasyl");

var storage = builder.AddAzureStorage("storage")
    .RunAsEmulator(emulator => emulator.WithDataVolume("zhasyl-azurite-data"));
var blobs = storage.AddBlobs("blobs");

var api = builder.AddProject<Projects.Zhasyl_Api>("api")
    .WithReference(postgres)
    .WaitFor(postgres)
    .WithReference(blobs)
    .WaitFor(blobs);

builder.AddJavaScriptApp("frontend", "../frontend", "dev")
    .WithReference(api)
    .WithEnvironment("API_BASE_URL", api.GetEndpoint("http"))
    .WithHttpEndpoint(port: 3000, env: "PORT")
    .WithExternalHttpEndpoints()
    .WaitFor(api);

builder.Build().Run();
