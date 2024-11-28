using Microsoft.FluentUI.AspNetCore.Components;
using IntelliPath.Shared.Models.Orchestrator;

namespace IntelliPath.Desktop.State;

public class AppState
{
    private DesignThemeModes _currentThemeMode = DesignThemeModes.Dark;
    private UserState _userState = new();
    private List<ConversationModel> _conversations = [];
    private ConversationModel? _currentConversation = new();

    public DesignThemeModes CurrentThemeMode
    {
        get => _currentThemeMode;
        set
        {
            _currentThemeMode = value;
            OnChange?.Invoke();
        }
    }

    public UserState GetUserState() => _userState;

    public void SetUserState(string displayName, string email)
    {
        _userState.UserDisplayName = displayName;
        _userState.UserEmail = email;
        OnChange?.Invoke();
    }

    public IReadOnlyList<ConversationModel> Conversations => _conversations.AsReadOnly();

    public void SetConversations(List<ConversationModel> conversations)
    {
        _conversations = conversations;
        OnChange?.Invoke();
    }

    public void ClearConversations()
    {
        _conversations.Clear();
        OnChange?.Invoke();
    }

    public ConversationModel? GetCurrentConversation() => _currentConversation;

    public void SetCurrentConversation(ConversationModel conversation)
    {
        _currentConversation = conversation;
        OnChange?.Invoke();
    }

    public void AddCurrentConversationToList(ConversationModel conversation)
    {
        _conversations.Add(conversation);
        OnChange?.Invoke();
    }

    public void AddMessageToCurrentConversation(ChatMessageModel message)
    {
        if (_currentConversation is not null)
        {
            _currentConversation.Messages.Add(message);
            OnChange?.Invoke();
        }
    }
    
    public void SetCurrentConversationId(string conversationId)
    {
        if (_currentConversation is not null)
        {
            _currentConversation.Id = conversationId;
            OnChange?.Invoke();
        }
    }

    public void SetCurrentConversationTitle(string title)
    {
        if (_currentConversation is not null)
        {
            _currentConversation.Title = title;
            OnChange?.Invoke();
        }
    }
    
    public void SetCurrentConversationCreatedAt(DateTime createdAt)
    {
        if (_currentConversation is not null)
        {
            _currentConversation.CreatedAt = createdAt;
            OnChange?.Invoke();
        }
    }
    
    public event Action? OnChange;

    public void DeleteConversation(string conversationId)
    {
        if (_conversations.Any(x => x.Id.ToUpperInvariant() == conversationId.ToUpperInvariant()))
        {
            var conversation = _conversations.First(x => x.Id.ToUpperInvariant() == conversationId.ToUpperInvariant());
            _conversations.Remove(conversation);
            OnChange?.Invoke();
        }
    }
}

public class UserState
{
    public string UserEmail { get; set; } = string.Empty;

    public string UserDisplayName { get; set; } = string.Empty;
}
