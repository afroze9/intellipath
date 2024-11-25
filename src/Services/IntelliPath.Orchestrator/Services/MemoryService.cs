using Microsoft.Extensions.VectorData;
using Microsoft.SemanticKernel.Embeddings;
using IntelliPath.Orchestrator.Entities.Vector;
using Qdrant.Client;
using Qdrant.Client.Grpc;

namespace IntelliPath.Orchestrator.Services;

public class MemoryService(
    [FromKeyedServices("memory-db")] IVectorStore vectorStore,
    QdrantClient client,
    ITextEmbeddingGenerationService textEmbeddingGenerationService) : IMemoryService
{
    private const string CollectionName = "memories";
    private async Task<IVectorStoreRecordCollection<ulong, Memory>> EnsureCreatedAsync(CancellationToken cancellationToken = default)
    {
        IVectorStoreRecordCollection<ulong, Memory> collection = vectorStore.GetCollection<ulong, Memory>(CollectionName);
        await collection.CreateCollectionIfNotExistsAsync(cancellationToken);
        return collection;
    }
    
    public async Task<List<Memory>> GetMemoriesAsync(CancellationToken cancellationToken = default)
    {
        await EnsureCreatedAsync(cancellationToken);
        ScrollResponse memories =
            await client.ScrollAsync(CollectionName, limit: 100, cancellationToken: cancellationToken);

        List<Memory> result = [];

        foreach (RetrievedPoint point in memories.Result)
        {
            PointId? id = point.Id;
            point.Payload.TryGetValue("memory_tags", out Value tagList);
            point.Payload.TryGetValue("memory_description", out Value description);
            
            if(id == null || tagList == null || description == null)
            {
                continue;
            }

            ListValue tagValues = tagList.ListValue;
            List<string> tags = [];
            
            foreach (Value value in tagValues.Values)
            {
                if (value.KindCase == Value.KindOneofCase.StringValue)
                {
                    tags.Add(value.StringValue);
                }
            }

            Memory memory = new ()
            {
                MemoryId = Guid.Parse(id.Uuid),
                Tags = tags,
                Description = description.StringValue
            };
            
            result.Add(memory);
        }

        return result;
    }

    public async Task SaveMemoryAsync(Memory memory, CancellationToken cancellationToken = default)
    {
        IVectorStoreRecordCollection<ulong, Memory> collection = await EnsureCreatedAsync(cancellationToken);
        memory.DescriptionEmbedding =
            await textEmbeddingGenerationService.GenerateEmbeddingAsync(memory.Description,
                cancellationToken: cancellationToken);
        
        await collection.UpsertAsync(memory, cancellationToken: cancellationToken);
    }
}

public interface IMemoryService
{
    Task<List<Memory>> GetMemoriesAsync(CancellationToken cancellationToken = default);

    Task SaveMemoryAsync(Memory memory, CancellationToken cancellationToken = default);
}