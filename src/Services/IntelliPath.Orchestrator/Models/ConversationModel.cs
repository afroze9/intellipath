namespace IntelliPath.Orchestrator.Models;

public class ConversationModel
{
    public required string Id { get; set; }
    public required DateTime CreatedAt { get; set; }
    public List<ChatMessageModel> Messages { get; set; } = [];
    public string? Title { get; set; }
}