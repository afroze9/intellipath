using IntelliPath.Shared.Models.Orchestrator;

namespace IntelliPath.Orchestrator.Models;

public class CreateConversationRequest
{
    public string? Id { get; set; }
    public List<CreateChatMessageRequest> Messages { get; set; } = [];
}

public static class CreateConversationRequestExtensions
{
    public static CreateConversationRequest ToCreateConversationRequest(this CreateConversationRequestModel model)
    {
        return new CreateConversationRequest
        {
            Messages = model.Messages.Select(x => x.ToCreateChatMessageRequest()).ToList(),
        };
    }
}