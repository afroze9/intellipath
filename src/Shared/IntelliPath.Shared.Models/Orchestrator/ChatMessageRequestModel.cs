namespace IntelliPath.Shared.Models.Orchestrator;

public class ChatMessageRequestModel
{
    public required string Id { get; set; }
    public required string Content { get; set; }
    public required string Role { get; set; }
}