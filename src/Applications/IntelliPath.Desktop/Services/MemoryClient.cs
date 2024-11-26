using System.Net.Http.Json;
using IntelliPath.Shared.Models.Orchestrator;

namespace IntelliPath.Desktop.Services;

public class MemoryClient(HttpClient memoryClient) : IMemoryClient
{
    public async Task<List<MemoryModel>> GetMemoriesAsync()
    {
        try
        {
            List<MemoryModel>? response = await memoryClient.GetFromJsonAsync<List<MemoryModel>>("/api/v1/memory");
            return response ?? [];
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            return [];
        }
    }

    public async Task SaveMemoryAsync(MemoryModel memoryModel)
    {
        await memoryClient.PostAsJsonAsync("/api/v1/memory", memoryModel);
    }

    public async Task<List<MemoryTagModel>> GetTagsAsync()
    {
        try
        {
            List<MemoryTagModel>? response = await memoryClient.GetFromJsonAsync<List<MemoryTagModel>>("/api/v1/memory/tags");
            return response ?? [];
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            return [];
        }
    }
}

public interface IMemoryClient
{
    Task<List<MemoryModel>> GetMemoriesAsync();

    Task SaveMemoryAsync(MemoryModel memoryModel);

    Task<List<MemoryTagModel>> GetTagsAsync();
}