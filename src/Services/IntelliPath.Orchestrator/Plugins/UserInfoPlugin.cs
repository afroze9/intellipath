using System.ComponentModel;
using IntelliPath.Orchestrator.Models;
using IntelliPath.Orchestrator.Services;
using Microsoft.Graph;
using Microsoft.Graph.Models;
using Microsoft.SemanticKernel;

namespace IntelliPath.Orchestrator.Plugins;

public class UserInfoPlugin(IGraphClientFactory graphClientFactory)
{
    [KernelFunction(name:"get_me")]
    [Description("Get information about the current user")]
    [return:Description("Information about the current user")]
    public async Task<UserModel?> GetMeAsync()
    {
        GraphServiceClient graphClient = graphClientFactory.Create();
        User? user = await graphClient.Me.GetAsync();
        if(user != null)
        {
            return new UserModel()
            {
                DisplayName = user.DisplayName,
                Department = user.Department,
                AboutMe = user.AboutMe,
                JobTitle = user.JobTitle,
            };
        }
        return null;
    }
}