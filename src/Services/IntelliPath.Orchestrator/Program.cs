using IntelliPath.Orchestrator.Data;
using IntelliPath.Orchestrator.Endpoints;
using IntelliPath.Orchestrator.Entities.Vector;
using IntelliPath.Orchestrator.Plugins;
using IntelliPath.Orchestrator.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.VectorData;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.Qdrant;
using Qdrant.Client;
using Scalar.AspNetCore;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();
builder.Services.AddProblemDetails();
builder.Services.AddOpenApi();

builder.Services
    .AddOpenAIChatCompletion("gpt-4o", builder.Configuration["OpenAiApiKey"] ?? string.Empty)
    .AddOpenAITextEmbeddingGeneration("text-embedding-3-large", builder.Configuration["OpenAiApiKey"] ?? string.Empty)
    .AddKernel();

builder.Services.AddHttpContextAccessor();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection"));
});
builder.Services.AddScoped<IChatService, ChatService>();
builder.Services.AddScoped<IGraphClientFactory, GraphClientFactory>();
builder.Services.AddScoped<IMemoryService, MemoryService>();
builder.AddQdrantClient("memory-db");
builder.Services.AddVectorStoreTextSearch<Memory>();
//builder.Services.AddQdrantVectorStore(serviceId: "memory-db");
builder.Services.AddKeyedSingleton<IVectorStore>(
    "memory-db",
    (sp, _) =>
    {
        QdrantClient qdrantClient = sp.GetRequiredService<QdrantClient>();
        return new QdrantVectorStore(qdrantClient);
    });


builder.Services.AddSingleton<KernelPlugin>(sp => KernelPluginFactory.CreateFromType<CalendarPlugin>(serviceProvider: sp));
builder.Services.AddSingleton<KernelPlugin>(sp => KernelPluginFactory.CreateFromType<DateTimePlugin>(serviceProvider: sp));
builder.Services.AddSingleton<KernelPlugin>(sp => KernelPluginFactory.CreateFromType<EmailPlugin>(serviceProvider: sp));
builder.Services.AddSingleton<KernelPlugin>(sp => KernelPluginFactory.CreateFromType<UserInfoPlugin>(serviceProvider: sp));

WebApplication app = builder.Build();
app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}
app.MapDefaultEndpoints();
app.MapChatEndpoints();
app.MapMemoryEndpoints();

using (IServiceScope scope = app.Services.CreateScope())
{
    ApplicationDbContext dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    dbContext.Database.Migrate();
}

app.Run();

