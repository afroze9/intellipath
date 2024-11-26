using System.Net.Http.Json;
using IntelliPath.Shared.Models.Orchestrator;

namespace IntelliPath.Desktop.Services;

public class ChatClient(HttpClient chatClient) : IChatClient
{
    public async Task<List<ConversationModel>> GetConversationsAsync()
    {
        try
        {
            List<ConversationModel>? response = await chatClient.GetFromJsonAsync<List<ConversationModel>>("api/v1/memory/tags");
            return response ?? [];
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            return [];
        }
    }
    
    public Task<ConversationModel?> GetConversationAsync(string conversationId)
    {
        throw new NotImplementedException();
    }

    public Task<List<ChatMessageModel>> GenerateAsync(ConversationModel? conversation)
    {
        throw new NotImplementedException();
    }
}

public interface IChatClient
{
    Task<ConversationModel?> GetConversationAsync(string conversationId);

    Task<List<ChatMessageModel>> GenerateAsync(ConversationModel? conversation);
}