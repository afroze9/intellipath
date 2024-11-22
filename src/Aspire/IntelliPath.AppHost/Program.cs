IDistributedApplicationBuilder builder = DistributedApplication.CreateBuilder(args);

var qdrantApiKey = builder.AddParameter("qdrantApiKey", secret: true);

IResourceBuilder<ProjectResource> elsaServer = builder
    .AddProject<Projects.IntelliPath_Server>("elsa-server");

builder.AddProject<Projects.IntelliPath_Studio>("elsa-studio")
    .WithExternalHttpEndpoints()
    .WithReference(elsaServer)
    .WaitFor(elsaServer);

var qdrant = builder.AddQdrant("memory-db", qdrantApiKey);

builder.Build().Run();
