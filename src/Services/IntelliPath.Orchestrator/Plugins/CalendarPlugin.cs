using System.ComponentModel;
using IntelliPath.Orchestrator.Models;
using IntelliPath.Orchestrator.Services;
using Microsoft.Graph;
using Microsoft.Graph.Models;
using Microsoft.SemanticKernel;

namespace IntelliPath.Orchestrator.Plugins;

public class CalendarPlugin(IGraphClient graphClient)
{
    [KernelFunction(name:"get_calendar_events")]
    [Description("Get information about the current user's calendar")]
    [return:Description("Information about the current user's calendar")]
    public async Task<List<EventModel>> GetCalendarEvents(DateTime startDateTime, DateTime endDateTime)
    {
        return await graphClient.GetCalendarEventsAsync(startDateTime, endDateTime);
    }
    
    [KernelFunction(name:"set_reminder")]
    [Description("Set a reminder for an event")]
    [return:Description("True if the reminder was set successfully, false otherwise")]
    public async Task<bool> SetReminderAsync(string subject, DateTime startDateTime, int durationInMinutes = 15, int reminderMinutesBeforeStart = 15)
    {
        return await graphClient.SetReminderAsync(subject, startDateTime, durationInMinutes, reminderMinutesBeforeStart);
    }
}