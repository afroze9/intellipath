namespace IntelliPath.Orchestrator.Entities;

public class ChatMessage
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public required string Content { get; set; }
    public required ChatMessageRole Role { get; set; }
}