using IntelliPath.Orchestrator.Entities;
using IntelliPath.Orchestrator.Entities.Vector;
using IntelliPath.Orchestrator.Services;
using IntelliPath.Shared.Models.Orchestrator;
using Microsoft.AspNetCore.Mvc;

namespace IntelliPath.Orchestrator.Endpoints;

public static class MemoryEndpoints
{
    private const string Tag = "Memory";
    private const string BasePath = "api/v1/memory";
    
    public static void MapMemoryEndpoints(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet(BasePath, GetAllAsync)
            .Produces<List<MemoryModel>>()
            .WithTags(Tag)
            .WithName("Get All");
        
        endpoints.MapPost(BasePath, AddAsync)
            .Produces<ChatMessageModel>()
            .WithTags(Tag)
            .WithName("Add");
        
        endpoints.MapGet(BasePath + "/tags", GetTagsAsync)
            .Produces<List<MemoryTagModel>>()
            .WithTags(Tag)
            .WithName("Get All Tags");
    }
    
    private static async Task<IResult> GetAllAsync(IMemoryService memoryService)
    {
        List<Memory> result = await memoryService.GetMemoriesAsync();
        List<MemoryModel> mappedResult = result.Select(x => new MemoryModel()
        {
            MemoryId = x.MemoryId,
            Tags = x.Tags,
            Description = x.Description,
        }).ToList();
        
        return Results.Ok(mappedResult);
    }

    private static async Task<IResult> GetTagsAsync(IMemoryService memoryService)
    {
        List<MemoryTag> tags = await memoryService.GetMemoryTagsAsync();
        List<MemoryTagModel> mappedTags = tags.Select(x => new MemoryTagModel()
        {
            TagId = x.Id,
            Name = x.Name,
        }).ToList();

        return Results.Ok(mappedTags);
    }
    
    private static async Task<IResult> AddAsync(
        [FromBody] MemoryModel model,
        IMemoryService memoryService)
    {
        Memory mappedMemory = new ()
        {
            MemoryId = Guid.NewGuid(),
            Tags = model.Tags,
            Description = model.Description
        };
        await memoryService.SaveMemoryAsync(mappedMemory);
        return Results.Created();
    }
}