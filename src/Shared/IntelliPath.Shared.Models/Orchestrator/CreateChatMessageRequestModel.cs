namespace IntelliPath.Shared.Models.Orchestrator;

public class CreateChatMessageRequestModel
{
    public required string Content { get; set; }
    public required string Role { get; set; }
}