﻿using IntelliPath.Orchestrator.Entities;
using IntelliPath.Shared.Models.Orchestrator;

namespace IntelliPath.Orchestrator.Mappers;

public static class ConversationMapper
{
    public static ConversationModel ToConversationModel(this Conversation conversation)
    {
        return new ConversationModel
        {
            Id = conversation.Id,
            Title = conversation.Title,
            Messages = conversation.Messages.Select(m => m.ToChatMessageModel()).ToList(),
        };
    }
    
    public static ChatMessageModel ToChatMessageModel(this ChatMessage message)
    {
        return new ChatMessageModel
        {
            Role = message.Role.ToString(),
            Content = message.Content,
        };
    }
}