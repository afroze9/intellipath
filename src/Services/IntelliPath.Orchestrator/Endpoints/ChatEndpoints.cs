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
        
        endpoints.MapPost($"{BasePath}/update", UpdateConversation)
            .Produces<bool>()
            .WithTags(Tag)
            .WithName("UpdateConversation");
        
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
        ConversationModel? result = await chatService.Generate(request.ToCreateConversationRequest());
        return result == null ? Results.InternalServerError() : Results.Ok(result);
    }
    
    private static async Task<IResult> UpdateConversation(
        [FromBody] UpdateConversationRequestModel model,
        IChatService chatService)
    {
        bool result = await chatService.UpdateConversation(model.ToUpdateConversationRequest());
        return result ? Results.Ok() : Results.BadRequest();
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