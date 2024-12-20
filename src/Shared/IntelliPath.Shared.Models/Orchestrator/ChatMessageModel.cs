namespace IntelliPath.Shared.Models.Orchestrator;

public class ChatMessageModel
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public DateTime CreatedAt { get; set; }
    public string ConversationId { get; set; } = string.Empty;
    public required string Content { get; set; }
    public required string Role { get; set; }
}