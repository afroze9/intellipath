using Microsoft.Extensions.VectorData;

namespace IntelliPath.Orchestrator.Entities.Vector;

public class Memory
{
    [VectorStoreRecordKey]
    public Guid MemoryId { get; set; } = Guid.NewGuid();
    
    [VectorStoreRecordData(IsFilterable = true, StoragePropertyName = "memory_tags")]
    public List<string> Tags { get; set; } = [];

    [VectorStoreRecordData(IsFullTextSearchable = true, StoragePropertyName = "memory_description")]
    public required string Description { get; set; }
    
    [VectorStoreRecordVector(3072, DistanceFunction.CosineSimilarity, IndexKind.Hnsw, StoragePropertyName = "memory_description_embedding")]
    public ReadOnlyMemory<float>? DescriptionEmbedding { get; set; }
}