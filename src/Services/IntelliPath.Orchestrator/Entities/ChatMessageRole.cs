namespace IntelliPath.Orchestrator.Entities;

public enum ChatMessageRole
{
    System = 0,
    User = 1,
    Assistant = 2,
    Tool = 3,
}

public static class ChatMessageRoleExtensions
{
    public static ChatMessageRole ToChatMessageRole(this string role)
    {
        return role switch
        {
            "System" => ChatMessageRole.System,
            "User" => ChatMessageRole.User,
            "Assistant" => ChatMessageRole.Assistant,
            "Tool" => ChatMessageRole.Tool,
            _ => throw new ArgumentException($"Invalid role: {role}"),
        };
    }
}