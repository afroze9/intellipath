IDistributedApplicationBuilder builder = DistributedApplication.CreateBuilder(args);

IResourceBuilder<ParameterResource> qdrantApiKey = builder.AddParameter("qdrantApiKey", secret: true);

IResourceBuilder<ProjectResource> elsaServer = builder
    .AddProject<Projects.IntelliPath_WorkflowServer>("elsa-server");

builder.AddProject<Projects.IntelliPath_WorkflowStudio>("elsa-studio")
    .WithExternalHttpEndpoints()
    .WithReference(elsaServer)
    .WaitFor(elsaServer);

IResourceBuilder<QdrantServerResource> qdrant = builder
    .AddQdrant("memory-db", qdrantApiKey)
    .WithLifetime(ContainerLifetime.Persistent);

builder.AddProject<Projects.IntelliPath_Orchestrator>("orchestrator")
    .WithReference(elsaServer)
    .WithReference(qdrant)
    .WaitFor(elsaServer)
    .WaitFor(qdrant);

builder.Build().Run();
