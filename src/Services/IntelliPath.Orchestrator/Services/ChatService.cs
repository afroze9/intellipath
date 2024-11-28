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
    private const string SystemPrompt =
        "You are an efficient assistant and respond with only whats needed and nothing else. Try and save important information to the knowledgebase as much as possible. And use it to help answer questions where needed.";


    public async Task<ConversationModel?> Generate(CreateConversationRequest request)
    {
        ChatMessageRequest? lastUserMessage =
            request.Messages.LastOrDefault(x => x.Role.ToChatMessageRole() == ChatMessageRole.User);

        if (lastUserMessage is null)
        {
            return null;
        }

        Conversation? conversation;

        if (string.IsNullOrEmpty(request.ConversationId))
        {
            conversation = new Conversation()
            {
                Title = await GenerateTitle(lastUserMessage.Content)
            };

            await context.Conversations.AddAsync(conversation);
            await context.SaveChangesAsync();
        }
        else
        {
            if (!Guid.TryParse(request.ConversationId, out Guid conversationId))
            {
                return null;
            }

            conversation = await context.Conversations.FirstOrDefaultAsync(x => x.Id == conversationId);
        }

        if (conversation == null)
        {
            return null;
        }

        ChatMessage userMessage = new ()
        {
            Conversation = conversation,
            Content = lastUserMessage.Content,
            Role = ChatMessageRole.User,
        };

        context.Messages.AddRange(userMessage);
        await context.SaveChangesAsync();

        IChatCompletionService completionService = kernel.GetRequiredService<IChatCompletionService>();
        OpenAIPromptExecutionSettings openAiPromptExecutionSettings = new ()
        {
            FunctionChoiceBehavior = FunctionChoiceBehavior.Auto(),
        };

        ChatHistory history = new ();
        history.AddSystemMessage(SystemPrompt);

        foreach (ChatMessageRequest message in request.Messages.Where(x =>
                     x.Role.ToChatMessageRole() == ChatMessageRole.User ||
                     x.Role.ToChatMessageRole() == ChatMessageRole.Assistant))
        {
            if (message.Role.ToChatMessageRole() == ChatMessageRole.Assistant)
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

        return new ConversationModel
        {
            Id = conversation.Id.ToString(),
            Messages =
            [
                new ChatMessageModel
                {
                    Id = Guid.NewGuid().ToString(),
                    ConversationId = conversation.Id.ToString(),
                    Role = ChatMessageRole.Assistant.ToString(),
                    Content = result.Content ?? "Error trying to get chat completion",
                    CreatedAt = DateTime.UtcNow,
                }
            ],
            Title = conversation.Title,
            CreatedAt = conversation.CreatedAt,
        };
    }

    private async Task<string> GenerateTitle(string message)
    {
        if (message.Length < 10)
        {
            return message;
        }

        IChatCompletionService completionService = kernel.GetRequiredService<IChatCompletionService>();
        OpenAIPromptExecutionSettings openAiPromptExecutionSettings = new ()
        {
            FunctionChoiceBehavior = FunctionChoiceBehavior.Auto(),
        };

        ChatHistory history = new ();
        history.AddSystemMessage(
            "Based on the given user message, generate a 5-10 word title for the chat and say nothing else");

        history.AddUserMessage(message);

        ChatMessageContent result = await completionService.GetChatMessageContentAsync(
            history,
            executionSettings: openAiPromptExecutionSettings,
            kernel: kernel);

        string title = result.Content ?? message;
        return title.TrimStart('"').TrimEnd('"');
    }

    public async Task<List<ConversationModel>> GetConversations()
    {
        List<ConversationModel> conversations = await context.Conversations
            .OrderByDescending(c => c.CreatedAt)
            .Take(50)
            .Select(c => new ConversationModel()
            {
                Id = c.Id.ToString(),
                CreatedAt = c.CreatedAt,
                Title = c.Title,
            })
            .ToListAsync();

        return conversations;
    }

    public async Task<ConversationModel?> GetConversationByIdAsync(string conversationId)
    {
        if (!Guid.TryParse(conversationId, out var conversationGuid))
        {
            return null;
        }

        Conversation? conversation = await context
            .Conversations
            .Where(x => x.Id == conversationGuid)
            .Include(x => x.Messages)
            .FirstOrDefaultAsync();
        return conversation?.ToConversationModel();

    }

    public async Task<bool> UpdateConversation(UpdateConversationRequest request)
    {
        if (string.IsNullOrEmpty(request.ConversationId))
        {
            return false;
        }

        if (!Guid.TryParse(request.ConversationId, out Guid conversationId))
        {
            return false;
        }

        Conversation? conversation = await context.Conversations.FirstOrDefaultAsync(x => x.Id == conversationId);

        if (conversation == null)
        {
            return false;
        }

        // Add messages to conversation
        if (request.Messages.Count > 0 && request.Messages[^1].Role.ToLowerInvariant() == "assistant")
        {
            if (request.Messages.Count > 1 && request.Messages[^2].Role.ToLowerInvariant() == "tool")
            {
                // Write the tool message first
                ChatMessageRequest toolMessageRequest = request.Messages[^2];
                ChatMessage toolMessage = new ChatMessage()
                {
                    Role = toolMessageRequest.Role.ToChatMessageRole(),
                    Content = toolMessageRequest.Content,
                    Conversation = conversation,
                    Id = Guid.Parse(toolMessageRequest.Id),
                };

                context.Messages.Add(toolMessage);
            }

            ChatMessageRequest assistantMessageRequest = request.Messages[^1];
            ChatMessage assistantMessage = new ChatMessage()
            {
                Role = assistantMessageRequest.Role.ToChatMessageRole(),
                Content = assistantMessageRequest.Content,
                Conversation = conversation,
                Id = Guid.Parse(assistantMessageRequest.Id),
            };

            context.Messages.Add(assistantMessage);

            await context.SaveChangesAsync();
        }
        else
        {
            return false;
        }

        return true;
    }
}

public interface IChatService
{
    Task<ConversationModel?> Generate(CreateConversationRequest request);

    Task<List<ConversationModel>> GetConversations();

    Task<ConversationModel?> GetConversationByIdAsync(string conversationId);

    Task<bool> UpdateConversation(UpdateConversationRequest request);
}