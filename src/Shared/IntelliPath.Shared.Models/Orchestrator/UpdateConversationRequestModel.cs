namespace IntelliPath.Shared.Models.Orchestrator;

public class UpdateConversationRequestModel
{
    public required string ConversationId { get; set; }
    
    public List<ChatMessageRequestModel> Messages { get; set; } = [];
}