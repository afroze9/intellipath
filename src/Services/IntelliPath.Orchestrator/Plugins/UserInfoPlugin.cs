using System.ComponentModel;
using IntelliPath.Orchestrator.Models;
using IntelliPath.Orchestrator.Services;
using Microsoft.SemanticKernel;

namespace IntelliPath.Orchestrator.Plugins;

public class UserInfoPlugin(IGraphClient graphClient)
{
    [KernelFunction(name:"get_me")]
    [Description("Get information about the current user")]
    [return:Description("Information about the current user")]
    public async Task<UserModel?> GetMeAsync(string select = "displayName,department,aboutMe,jobTitle")
    {
        return await graphClient.GetMeAsync();
    }
}