var builder = DistributedApplication.CreateBuilder(args);
var authSecret = builder.AddParameter(
    "auth-secret",
    () => Convert.ToBase64String(System.Security.Cryptography.RandomNumberGenerator.GetBytes(48)),
    publishValueAsDefault: false,
    secret: true);
var ephemeralInfrastructure = bool.TryParse(
    builder.Configuration["Zhasyl:EphemeralInfrastructure"],
    out var configuredEphemeralInfrastructure) && configuredEphemeralInfrastructure;

var postgresServer = builder.AddPostgres("postgres").WithImageTag("17");
if (!ephemeralInfrastructure)
{
    postgresServer.WithDataVolume("zhasyl-postgres-data");
}
var postgres = postgresServer.AddDatabase("zhasyl");

var storage = builder.AddAzureStorage("storage");
if (ephemeralInfrastructure)
{
    storage.RunAsEmulator();
}
else
{
    storage.RunAsEmulator(emulator => emulator.WithDataVolume("zhasyl-azurite-data"));
}
var blobs = storage.AddBlobs("blobs");

var api = builder.AddProject<Projects.Zhasyl_Api>("api")
    .WithReference(postgres)
    .WaitFor(postgres)
    .WithReference(blobs)
    .WaitFor(blobs);

builder.AddJavaScriptApp("frontend", "../frontend", "dev")
    .WithReference(api)
    .WithEnvironment("API_BASE_URL", api.GetEndpoint("http"))
    .WithEnvironment("AUTH_SECRET", authSecret)
    .WithHttpEndpoint(port: 3000, env: "PORT")
    .WithExternalHttpEndpoints()
    .WaitFor(api);

builder.Build().Run();
