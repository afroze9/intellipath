IDistributedApplicationBuilder builder = DistributedApplication.CreateBuilder(args);

IResourceBuilder<ProjectResource> elsaServer = builder
    .AddProject<Projects.IntelliPath_Server>("elsa-server");

builder.AddProject<Projects.IntelliPath_Studio>("elsa-studio")
    .WithExternalHttpEndpoints()
    .WithReference(elsaServer)
    .WaitFor(elsaServer);

builder.Build().Run();
