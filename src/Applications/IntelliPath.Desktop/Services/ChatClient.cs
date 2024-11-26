using System.Net.Http.Json;
using IntelliPath.Desktop.State;
using IntelliPath.Shared.Models;
using IntelliPath.Shared.Models.Orchestrator;

namespace IntelliPath.Desktop.Services;

public class ChatClient(HttpClient chatClient, AppState appState) : IChatClient
{
    public async Task<List<ConversationModel>> GetConversationsAsync()
    {
        try
        {
            List<ConversationModel>? response = await chatClient.GetFromJsonAsync<List<ConversationModel>>("api/v1/chat/conversation");
            return response ?? [];
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            return [];
        }
    }
    
    public async Task<ConversationModel?> GetConversationByIdAsync(string conversationId)
    {
        try
        {
            ConversationModel? conversation = await chatClient.GetFromJsonAsync<ConversationModel>("api/v1/chat/conversation/" + conversationId);
            return conversation;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return null;
        }
    }

    public async Task<ConversationModel?> GenerateAsync(ConversationModel conversation)
    {
        try
        {
            CreateConversationRequestModel request = new CreateConversationRequestModel
            {
                Messages = conversation.Messages
                    .Where(m => m.Role.ToLowerInvariant() != "system")
                    .Select(m => new CreateChatMessageRequestModel
                    {
                        Content = m.Content,
                        Role = m.Role,
                    }).ToList(),
            };
            string token = appState.GetUserState().AuthToken;
            chatClient.DefaultRequestHeaders.Add(IntelliPathHeaders.GraphTokenHeader, token);
            HttpResponseMessage response = await chatClient.PostAsJsonAsync("api/v1/chat/generate", request);
            if(response.IsSuccessStatusCode)
            {
                ConversationModel? conversationResponse = await response.Content.ReadFromJsonAsync<ConversationModel>();
                return conversationResponse;
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }

        return null;
    }

    public async Task<string> GenerateTitleAsync(string conversationId, string message)
    {
        try
        {
            HttpResponseMessage response = await chatClient.PostAsJsonAsync($"api/v1/chat/title?conversationId={conversationId}", message);
            if (response.IsSuccessStatusCode)
            {
                ChatMessageModel? responseModel = await response.Content.ReadFromJsonAsync<ChatMessageModel>();
                return responseModel?.Content ?? message;
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
        
        return message;
    }
}

public interface IChatClient
{
    Task<List<ConversationModel>> GetConversationsAsync();
    Task<ConversationModel?> GetConversationByIdAsync(string conversationId);
    Task<ConversationModel?> GenerateAsync(ConversationModel conversation);
    Task<string> GenerateTitleAsync(string conversationId, string message);
}