using IntelliPath.Orchestrator.Data;
using IntelliPath.Orchestrator.Entities;
using IntelliPath.Orchestrator.Models;
using IntelliPath.Shared.Models.Orchestrator;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;

namespace IntelliPath.Orchestrator.Services;

public class ChatService(Kernel kernel, ApplicationDbContext context) : IChatService
{
    public async Task<ChatMessageModel> Generate(CreateConversationRequest request)
    {
        IChatCompletionService completionService = kernel.GetRequiredService<IChatCompletionService>();
        OpenAIPromptExecutionSettings openAiPromptExecutionSettings = new ()
        {
            FunctionChoiceBehavior = FunctionChoiceBehavior.Auto(),
        };

        ChatHistory history = new ();
        history.AddSystemMessage("You are an efficient assistant and respond with only whats needed and nothing else. Try and save important information to the knowledgebase as much as possible. And use it to help answer questions where needed.");

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

        return new ChatMessageModel{
            Role = ChatMessageRole.Assistant.ToString(),
            Content = result.Content ?? "Error trying to get chat completion",
        };
    }

    public async Task<ChatMessageModel> GenerateTitle(string message)
    {
        if (message.Length < 10)
        {
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

        return new ChatMessageModel
        {
            Role = ChatMessageRole.Assistant.ToString(),
            Content = title,
        };
    }
}

public interface IChatService
{
    Task<ChatMessageModel> Generate(CreateConversationRequest request);

    Task<ChatMessageModel> GenerateTitle(string message);
}