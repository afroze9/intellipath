using IntelliPath.Shared.Models.Orchestrator;

namespace IntelliPath.Orchestrator.Models;

public class ChatMessageRequest
{
    public string Id { get; set; } = string.Empty;

    public string Content { get; set; } = string.Empty;

    public string Role { get; set; } = string.Empty;
}

public static class CreateChatMessageRequestExtensions
{
    public static ChatMessageRequest ToCreateChatMessageRequest(this ChatMessageRequestModel model)
    {
        return new ChatMessageRequest
        {
            Content = model.Content,
            Role = model.Role,
            Id = model.Id,
        };
    }
}