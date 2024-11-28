namespace IntelliPath.Orchestrator.Entities;

public abstract class EntityBase
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public DateTime CreatedAt { get; set; }
    public DateTime ModifiedAt { get; set; }
}