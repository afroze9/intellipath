namespace IntelliPath.Orchestrator.Models;

public class UpdateConversationRequest
{
    public required string ConversationId { get; set; }
    
    public List<ChatMessageRequest> Messages { get; set; } = new();
}