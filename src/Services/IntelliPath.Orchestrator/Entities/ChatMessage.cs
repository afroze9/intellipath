using System.ComponentModel.DataAnnotations;

namespace IntelliPath.Orchestrator.Entities;

public class ChatMessage : EntityBase
{
    [Required]
    public virtual Conversation Conversation { get; set; }

    [Required]
    public string Content { get; set; } = string.Empty;
    public required ChatMessageRole Role { get; set; }
}