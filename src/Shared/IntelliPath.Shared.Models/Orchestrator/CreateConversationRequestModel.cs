namespace IntelliPath.Shared.Models.Orchestrator;

public class CreateConversationRequestModel
{
    public List<CreateChatMessageRequestModel> Messages { get; set; } = [];
}
