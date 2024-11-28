using IntelliPath.Shared.Models.Orchestrator;

namespace IntelliPath.Orchestrator.Models;

public class CreateConversationRequest
{
    public string? ConversationId { get; set; }
    public List<ChatMessageRequest> Messages { get; set; } = [];
}

public static class CreateConversationRequestExtensions
{
    public static CreateConversationRequest ToCreateConversationRequest(this CreateConversationRequestModel model)
    {
        return new CreateConversationRequest
        {
            ConversationId = model.ConversationId,
            Messages = model.Messages.Select(x => x.ToCreateChatMessageRequest()).ToList(),
        };
    }
    
    public static UpdateConversationRequest ToUpdateConversationRequest(this UpdateConversationRequestModel model)
    {
        return new UpdateConversationRequest
        {
            ConversationId = model.ConversationId,
            Messages = model.Messages.Select(x => x.ToCreateChatMessageRequest()).ToList(),
        };
    }
}