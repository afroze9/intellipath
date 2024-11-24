using System.ComponentModel;
using IntelliPath.Orchestrator.Models;
using IntelliPath.Orchestrator.Services;
using Microsoft.Graph;
using Microsoft.Graph.Models;
using Microsoft.SemanticKernel;

namespace IntelliPath.Orchestrator.Plugins;

public class CalendarPlugin(IGraphClientFactory graphClientFactory)
{
    [KernelFunction(name:"get_calendar_events")]
    [Description("Get information about the current user's calendar")]
    [return:Description("Information about the current user's calendar")]
    public async Task<List<EventModel>> GetCalendarEvents(DateTime startDateTime, DateTime endDateTime)
    {
        GraphServiceClient graphClient = graphClientFactory.Create();

        EventCollectionResponse? events = await graphClient.Me
            .CalendarView
            .GetAsync(options =>
            {
                options.QueryParameters.StartDateTime = startDateTime.ToString("o");
                options.QueryParameters.EndDateTime = endDateTime.ToString("o");
            });

        List<EventModel> eventModels = [];
        if (events?.Value?.Count > 0)
        {
            foreach (var @event in events.Value)
            {
                eventModels.Add(new EventModel()
                {
                    Subject = @event.Subject,
                    Start = @event.Start?.DateTime != null ? TimeZoneInfo.ConvertTimeFromUtc(DateTime.Parse(@event.Start.DateTime), TimeZoneInfo.Local) : null,
                    End = @event.End?.DateTime != null ? TimeZoneInfo.ConvertTimeFromUtc(DateTime.Parse(@event.End.DateTime), TimeZoneInfo.Local) : null,
                });
            }
        }

        return eventModels;
    }
    
    [KernelFunction(name:"set_reminder")]
    [Description("Set a reminder for an event")]
    [return:Description("True if the reminder was set successfully, false otherwise")]
    public async Task<bool> SetReminderAsync(string subject, DateTime startDateTime, int durationInMinutes = 15, int reminderMinutesBeforeStart = 15)
    {
        GraphServiceClient graphClient = graphClientFactory.Create();

        var @event = new Event
        {
            Subject = subject,
            Start = new DateTimeTimeZone
            {
                DateTime = startDateTime.ToString("o"),
                TimeZone = "UTC"
            },
            End = new DateTimeTimeZone
            {
                DateTime = startDateTime.AddMinutes(durationInMinutes).ToString("o"),
                TimeZone = "UTC"
            },
            ReminderMinutesBeforeStart = reminderMinutesBeforeStart
        };

        try
        {
            await graphClient.Me.Events.PostAsync(@event);
            return true;
        }
        catch (Exception ex)
        {
            // Handle exception
            Console.WriteLine($"Error setting reminder: {ex.Message}");
            return false;
        }
    }
}