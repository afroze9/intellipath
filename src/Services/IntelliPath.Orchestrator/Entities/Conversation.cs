using System.ComponentModel.DataAnnotations;

namespace IntelliPath.Orchestrator.Entities;

public class Conversation : EntityBase
{
    [Required]
    [MaxLength(255)]
    public string Title { get; set; } = string.Empty;
    public List<ChatMessage> Messages { get; set; } = [];
}