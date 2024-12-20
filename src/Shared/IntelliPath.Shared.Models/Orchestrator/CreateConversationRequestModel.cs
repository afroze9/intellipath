namespace IntelliPath.Shared.Models.Orchestrator;

public class CreateConversationRequestModel
{
    public string? ConversationId { get; set; }
    public List<ChatMessageRequestModel> Messages { get; set; } = [];
}