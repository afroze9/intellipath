using System.Net.Http.Json;
using IntelliPath.Desktop.State;
using IntelliPath.Shared.Models;
using IntelliPath.Shared.Models.Orchestrator;

namespace IntelliPath.Desktop.Services;

public class ChatClient(HttpClient chatClient, IAuthService authService) : IChatClient
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
                ConversationId = conversation.Id,
                Messages = conversation.Messages
                    .Where(m => m.Role.ToLowerInvariant() != "system")
                    .Select(m => new ChatMessageRequestModel
                    {
                        Id = m.Id,
                        Content = m.Content,
                        Role = m.Role,
                    }).ToList(),
            };

            string token = await authService.GetTokenAsync();
            if(chatClient.DefaultRequestHeaders.Contains(IntelliPathHeaders.GraphTokenHeader))
            {
                chatClient.DefaultRequestHeaders.Remove(IntelliPathHeaders.GraphTokenHeader);
            }
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
    
    public async Task<bool> UpdateConversationAsync(ConversationModel model)
    {
        try
        {
            UpdateConversationRequestModel request = new UpdateConversationRequestModel()
            {
                ConversationId = model.Id,
                Messages = model.Messages
                    .Where(m => m.Role.ToLowerInvariant() != "system")
                    .Select(m => new ChatMessageRequestModel
                    {
                        Id = m.Id,
                        Content = m.Content,
                        Role = m.Role,
                    }).ToList(),
            };
            
            HttpResponseMessage response = await chatClient.PostAsJsonAsync("api/v1/chat/update", request);
            return response.IsSuccessStatusCode;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
        return false;
    }
}

public interface IChatClient
{
    Task<List<ConversationModel>> GetConversationsAsync();
    Task<ConversationModel?> GetConversationByIdAsync(string conversationId);
    Task<ConversationModel?> GenerateAsync(ConversationModel conversation);
    Task<bool> UpdateConversationAsync(ConversationModel model);
}