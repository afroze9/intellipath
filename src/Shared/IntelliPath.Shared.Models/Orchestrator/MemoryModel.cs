namespace IntelliPath.Shared.Models.Orchestrator;

public class MemoryModel
{
    public Guid MemoryId { get; set; }
    
    public List<string> Tags { get; set; } = [];

    public required string Description { get; set; }
}