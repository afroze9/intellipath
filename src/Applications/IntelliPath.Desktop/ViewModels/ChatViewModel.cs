using System.ComponentModel.DataAnnotations;

namespace IntelliPath.Desktop.ViewModels;

public class ChatViewModel
{
    [Required] public string ChatInput { get; set; } = string.Empty;
}