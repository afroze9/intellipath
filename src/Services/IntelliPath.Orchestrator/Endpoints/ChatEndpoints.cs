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
        endpoints.MapPost(BasePath + "/generate", GenerateAsync)
            .Produces<ConversationModel>()
            .WithTags(Tag)
            .WithName("Generate");
        
        endpoints.MapPost(BasePath + "/title", GenerateTitleAsync)
            .Produces<ChatMessageModel>()
            .WithTags(Tag)
            .WithName("GenerateTitle");
        
        endpoints.MapGet(BasePath + "/conversation", GetConversationsAsync)
            .Produces<List<ConversationModel>>()
            .WithTags(Tag)
            .WithName("GetConversations");
        
        endpoints.MapGet(BasePath + "/conversation/{conversationId}", GetConversationByIdAsync)
            .Produces<ConversationModel?>()
            .WithTags(Tag)
            .WithName("GetConversationById");
    }
    
    private static async Task<IResult> GenerateAsync(
        [FromBody] CreateConversationRequestModel request,
        IChatService chatService)
    {
        ConversationModel result = await chatService.Generate(request.ToCreateConversationRequest());
        return Results.Ok(result);
    }
    
    private static async Task<IResult> GenerateTitleAsync(
        [FromQuery] string conversationId,
        [FromBody] string message,
        IChatService chatService)
    {
        ChatMessageModel result = await chatService.GenerateTitle(conversationId, message);
        return Results.Ok(result);
    }
    
    private static async Task<IResult> GetConversationsAsync(IChatService chatService)
    {
        List<ConversationModel> result = await chatService.GetConversations();
        return Results.Ok(result);
    }
    
    private static async Task<IResult> GetConversationByIdAsync(
        IChatService chatService,
        [FromRoute] string conversationId)
    {
        ConversationModel? result = await chatService.GetConversationByIdAsync(conversationId);
        return result == null ? Results.NotFound() : Results.Ok(result);
    }
}