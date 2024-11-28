namespace IntelliPath.Orchestrator.Models;

public class EmailSummaryModel
{
    public string? Id { get; set; }
    public string? ConversationId { get; set; }
    public string? Subject { get; set; }
    public string? From { get; set; }
    public DateTimeOffset? ReceivedDateTime { get; set; }
}