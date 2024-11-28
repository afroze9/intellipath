using System.Net.Http.Headers;
using IntelliPath.Orchestrator.Models;
using IntelliPath.Shared.Models;
using Microsoft.Graph;
using Microsoft.Graph.Models;

namespace IntelliPath.Orchestrator.Services;

public class GraphHttpClient(
    HttpClient client,
    IHttpContextAccessor httpContextAccessor,
    ILogger<GraphHttpClient> logger)
    : IGraphClient
{
    private void PopulateToken()
    {
        string token =
            httpContextAccessor.HttpContext?.Request.Headers[IntelliPathHeaders.GraphTokenHeader].ToString() ??
            string.Empty;
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
    }

    public async Task<UserModel?> GetMeAsync(string select = "displayName,department,aboutMe,jobTitle")
    {
        PopulateToken();
        try
        {
            UserModel? response = await client.GetFromJsonAsync<UserModel>($"v1.0/me?$select={select}");
            return response;
        }
        catch (Exception e)
        {
            logger.LogWarning("Failed to get user information from Graph API. Error: {Error}", e.Message);
            Console.WriteLine(e);
        }

        return null;
    }

    public async Task<List<EventModel>> GetCalendarEventsAsync(DateTime startDateTime, DateTime endDateTime,
        string select = "subject,start,end")
    {
        PopulateToken();
        try
        {
            var requestUrl = $"v1.0/me/calendarView?startDateTime={startDateTime:yyyy-MM-ddTHH:mm:ss.fffZ}&endDateTime={endDateTime:yyyy-MM-ddTHH:mm:ss.fffZ}&$select={select}";
            EventCollectionResponse? events = await client.GetFromJsonAsync<EventCollectionResponse>(requestUrl);
            List<EventModel> eventModels = [];
            if (events?.Value?.Count > 0)
            {
                foreach (Event @event in events.Value)
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
        catch (Exception e)
        {
            logger.LogWarning("Failed to get calendar events from Graph API. Error: {Error}", e.Message);
            Console.WriteLine(e);
        }

        return [];
    }

    public async Task<List<EmailSummaryModel>> GetEmailsAsync(DateTime startDateTime, DateTime endDateTime,
        string select = "id,conversationId,subject,from,receivedDateTime")
    {
        PopulateToken();
        try
        {
            MessageCollectionResponse? response = await client.GetFromJsonAsync<MessageCollectionResponse>(
                $"v1.0/me/messages?$filter=receivedDateTime ge {startDateTime:yyyy-MM-ddTHH:mm:ss.fffZ} and receivedDateTime le {endDateTime:yyyy-MM-ddTHH:mm:ss.fffZ}&$top=25&$select={select}");
            return response?.Value?.Select(mail =>
                new EmailSummaryModel()
                {
                    Id = mail.Id,
                    ConversationId = mail.ConversationId,
                    Subject = mail.Subject,
                    From = mail.From?.EmailAddress?.Address ?? string.Empty,
                    ReceivedDateTime = mail.ReceivedDateTime
                }
            ).ToList() ?? [];
        }
        catch (Exception e)
        {
            logger.LogWarning("Failed to get emails from Graph API. Error: {Error}", e.Message);
            Console.WriteLine(e);
        }

        return [];
    }
    
    public async Task<bool> SetReminderAsync(string subject, DateTime startDateTime, int durationInMinutes = 15, int reminderMinutesBeforeStart = 15)
    {
        PopulateToken();
        try
        {
            Event @event = new Event
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
            await client.PostAsJsonAsync("v1.0/me/events", @event);
            return true;
        }
        catch (Exception e)
        {
            logger.LogWarning("Failed to set reminder from Graph API. Error: {Error}", e.Message);
            Console.WriteLine(e);
        }

        return false;
    }
}

public interface IGraphClient
{
    Task<UserModel?> GetMeAsync(string select = "displayName,department,aboutMe,jobTitle");

    Task<List<EventModel>> GetCalendarEventsAsync(DateTime startDateTime, DateTime endDateTime,
        string select = "subject,start,end");

    Task<List<EmailSummaryModel>> GetEmailsAsync(DateTime startDateTime, DateTime endDateTime,
        string select = "id,conversationId,subject,from,receivedDateTime");

    Task<bool> SetReminderAsync(string subject, DateTime startDateTime, int durationInMinutes = 15, int reminderMinutesBeforeStart = 15);
}