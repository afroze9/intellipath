namespace IntelliPath.Orchestrator.Entities;

public class MemoryTag
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public required string Name { get; set; }
}