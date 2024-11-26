using IntelliPath.Orchestrator.Models;
using IntelliPath.Orchestrator.Services;
using IntelliPath.Shared.Models.Orchestrator;
using Microsoft.AspNetCore.Mvc;
using ChatMessageModel = IntelliPath.Shared.Models.Orchestrator.ChatMessageModel;

namespace IntelliPath.Orchestrator.Endpoints;

public static class ChatEndpoints
{
    private const string Tag = "Chat";
    private const string BasePath = "api/v1/chat";
    
    public static void MapChatEndpoints(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapPost(BasePath, GenerateAsync)
            .Produces<ChatMessageModel>()
            .WithTags(Tag)
            .WithName("Generate");
        
        endpoints.MapPost(BasePath + "/title", GenerateTitleAsync)
            .Produces<ChatMessageModel>()
            .WithTags(Tag)
            .WithName("GenerateTitle");
    }
    
    private static async Task<IResult> GenerateAsync(
        [FromBody] CreateConversationRequestModel request,
        IChatService chatService)
    {
        ChatMessageModel result = await chatService.Generate(request.ToCreateConversationRequest());
        return Results.Ok(result);
    }
    
    private static async Task<IResult> GenerateTitleAsync(
        [FromBody] string message,
        IChatService chatService)
    {
        ChatMessageModel result = await chatService.GenerateTitle(message);
        return Results.Ok(result);
    }
}

public static class ConversationEndpoints
{
    private const string Tag = "Conversation";
    private const string BasePath = "api/v1/conversation";

    public static void MapConversationEndpoints(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet(BasePath, GetConversations)
            .Produces<List<ConversationModel>>()
            .WithTags(Tag)
            .WithName("GetConversations");
    }

    private static async Task<IResult> GetConversations(IChatService chatService)
    {
        List<ConversationModel> result = await chatService.GetConversations();
        return Results.Ok(result);
    }
}