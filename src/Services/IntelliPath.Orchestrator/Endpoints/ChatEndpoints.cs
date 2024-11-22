namespace IntelliPath.Orchestrator.Endpoints;

public static class ChatEndpoints
{
    private const string Tag = "Chat";
    private const string BasePath = "api/v1/chats";
    
    public static void MapChatEndpoints(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet(BasePath, GenerateAsync).WithTags(Tag).WithName("Generate");
    }
    
    private static async Task<IResult> GenerateAsync()
    {
        return Results.Ok<string>("done");
    }
}