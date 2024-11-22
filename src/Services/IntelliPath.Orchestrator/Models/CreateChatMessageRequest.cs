using IntelliPath.Orchestrator.Entities;
using IntelliPath.Shared.Models;
using IntelliPath.Shared.Models.Orchestrator;

namespace IntelliPath.Orchestrator.Models;

public class CreateChatMessageRequest
{
    public required string Content { get; set; }
    public required ChatMessageRole Role { get; set; }
}

public static class CreateChatMessageRequestExtensions
{
    public static CreateChatMessageRequest ToCreateChatMessageRequest(this CreateChatMessageRequestModel model)
    {
        return new CreateChatMessageRequest
        {
            Content = model.Content,
            Role = model.Role.ToChatMessageRole(),
        };
    }
}