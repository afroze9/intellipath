using System.ComponentModel;
using IntelliPath.Orchestrator.Models;
using IntelliPath.Orchestrator.Services;
using Microsoft.Graph;
using Microsoft.Graph.Models;
using Microsoft.SemanticKernel;

namespace IntelliPath.Orchestrator.Plugins;

public class EmailPlugin(IGraphClientFactory graphClientFactory)
{
    [KernelFunction(name:"get_emails")]
    [Description("Get emails for the current user")]
    [return:Description("List of emails for the current user")]
    public async Task<List<EmailSummaryModel>> GetEmailsAsync(DateTime startDateTime, DateTime endDateTime)
    {
        GraphServiceClient graphClient = graphClientFactory.Create();

        MessageCollectionResponse? messages = await graphClient.Me.Messages.GetAsync(options =>
        {
            options.QueryParameters.Filter = $"receivedDateTime ge {startDateTime:o} and receivedDateTime le {endDateTime:o}";
            options.QueryParameters.Top = 25;
        });
        
        if (messages?.Value is null)
        {
            return [];
        }

        List<EmailSummaryModel> emails = [];
        foreach (Message message in messages.Value)
        {
            emails.Add(new EmailSummaryModel()
            {
                MessageId = message.Id,
                ConversationId = message.ConversationId,
                Subject = message.Subject,
                From = message.Sender?.EmailAddress?.Address,
                ReceivedDateTime = message.ReceivedDateTime
            });
        }

        return emails;
    }
}