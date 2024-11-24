namespace IntelliPath.Orchestrator.Models;

public class EventModel
{
    public string? Subject { get; set; }
    public DateTime? Start { get; set; }
    public DateTime? End { get; set; }
    
    public int DurationInMinutes => Start.HasValue && End.HasValue ? (int)End.Value.Subtract(Start.Value).TotalMinutes : 0;
}