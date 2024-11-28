namespace IntelliPath.Shared.Models.Orchestrator;

public class ConversationModel
{
    public string Id { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public List<ChatMessageModel> Messages { get; set; } = [];
    public string? Title { get; set; }
}