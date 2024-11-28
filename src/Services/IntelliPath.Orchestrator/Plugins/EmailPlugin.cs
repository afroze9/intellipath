using System.ComponentModel;
using IntelliPath.Orchestrator.Models;
using IntelliPath.Orchestrator.Services;
using Microsoft.SemanticKernel;

namespace IntelliPath.Orchestrator.Plugins;

public class EmailPlugin(IGraphClient graphClient)
{
    [KernelFunction(name:"get_emails")]
    [Description("Get emails for the current user")]
    [return:Description("List of emails for the current user")]
    public async Task<List<EmailSummaryModel>> GetEmailsAsync(DateTime startDateTime, DateTime endDateTime)
    {
        return await graphClient.GetEmailsAsync(startDateTime, endDateTime);
    }
}