namespace IntelliPath.Orchestrator.Entities;

public class Conversation
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public List<ChatMessage> Messages { get; set; } = [];
    public string? Title { get; set; }
}