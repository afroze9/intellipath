using IntelliPath.Orchestrator.Data;
using IntelliPath.Orchestrator.Entities;
using IntelliPath.Orchestrator.Mappers;
using IntelliPath.Orchestrator.Models;
using IntelliPath.Shared.Models.Orchestrator;
using Microsoft.EntityFrameworkCore;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;

namespace IntelliPath.Orchestrator.Services;

public class ChatService(Kernel kernel, ApplicationDbContext context) : IChatService
{
    private const string SystemPrompt = "You are an efficient assistant and respond with only whats needed and nothing else. Try and save important information to the knowledgebase as much as possible. And use it to help answer questions where needed.";
    public async Task<ConversationModel> Generate(CreateConversationRequest request)
    {
        ConversationModel response = new ConversationModel();
        if(request.Id == null || !await context.Conversations.AnyAsync(x => x.Id == request.Id))
        {
            Conversation conversationToAdd = new Conversation()
            {
                Messages = request.Messages
                    .Select(m => new ChatMessage()
                {
                    Content = m.Content,
                    Role = m.Role,
                }).ToList(),
                CreatedAt = DateTime.UtcNow,
            };

            await context.Conversations.AddAsync(conversationToAdd);
            await context.SaveChangesAsync();
            
            response.Id = conversationToAdd.Id;
            response.CreatedAt = conversationToAdd.CreatedAt;
        }
        
        IChatCompletionService completionService = kernel.GetRequiredService<IChatCompletionService>();
        OpenAIPromptExecutionSettings openAiPromptExecutionSettings = new ()
        {
            FunctionChoiceBehavior = FunctionChoiceBehavior.Auto(),
        };

        ChatHistory history = new ();
        history.AddSystemMessage(SystemPrompt);

        foreach (CreateChatMessageRequest message in request.Messages)
        {
            if(message.Role == ChatMessageRole.Assistant)
            {
                history.AddAssistantMessage(message.Content);
            }
            else
            {
                history.AddUserMessage(message.Content);
            }
        }
        
        ChatMessageContent result = await completionService.GetChatMessageContentAsync(
            history,
            executionSettings: openAiPromptExecutionSettings,
            kernel: kernel);

        response.Messages.Add(new ChatMessageModel
        {
            Role = ChatMessageRole.Assistant.ToString(),
            Content = result.Content ?? "Error trying to get chat completion",
        });
        
        return response;
    }

    public async Task<ChatMessageModel> GenerateTitle(string conversationId, string message)
    {
        Conversation? conversationToUpdate = await context.Conversations.FindAsync(conversationId);
        
        if (message.Length < 10)
        {
            if (conversationToUpdate != null)
            {
                conversationToUpdate.Title = message;
                await context.SaveChangesAsync();
            }
            return new ChatMessageModel { Content = message, Role = ChatMessageRole.Assistant.ToString(), };
        }
        
        IChatCompletionService completionService = kernel.GetRequiredService<IChatCompletionService>();
        OpenAIPromptExecutionSettings openAiPromptExecutionSettings = new ()
        {
            FunctionChoiceBehavior = FunctionChoiceBehavior.Auto(),
        };

        ChatHistory history = new ();
        history.AddSystemMessage("Based on the given user message, generate a 5-10 word title for the chat and say nothing else");
        history.AddUserMessage(message);
        
        ChatMessageContent result = await completionService.GetChatMessageContentAsync(
            history,
            executionSettings: openAiPromptExecutionSettings,
            kernel: kernel);
        
        string title = result.Content ?? message;
        title = title.TrimStart('"').TrimEnd('"');
        
        if (conversationToUpdate != null)
        {
            conversationToUpdate.Title = title;
            await context.SaveChangesAsync();
        }

        return new ChatMessageModel
        {
            Role = ChatMessageRole.Assistant.ToString(),
            Content = title,
        };
    }

    public async Task<List<ConversationModel>> GetConversations()
    {
        List<ConversationModel> conversations = await context.Conversations
            .OrderByDescending(c => c.CreatedAt)
            .Take(50)
            .Select(c => new ConversationModel()
            {
                Id = c.Id,
                CreatedAt = c.CreatedAt,
                Title = c.Title,
            })
            .ToListAsync();
        
        return conversations;
    }

    public async Task<ConversationModel?> GetConversationByIdAsync(string conversationId)
    {
        Conversation? conversation = await context.Conversations.FindAsync(conversationId);
        return conversation?.ToConversationModel();
    }
}

public interface IChatService
{
    Task<ConversationModel> Generate(CreateConversationRequest request);

    Task<ChatMessageModel> GenerateTitle(string conversationId, string message);

    Task<List<ConversationModel>> GetConversations();
    Task<ConversationModel?> GetConversationByIdAsync(string conversationId);
}