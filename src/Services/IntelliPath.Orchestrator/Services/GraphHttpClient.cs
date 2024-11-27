using System.Net.Http.Headers;
using IntelliPath.Orchestrator.Models;
using IntelliPath.Shared.Models;

namespace IntelliPath.Orchestrator.Services;

public class GraphHttpClient(
    HttpClient client,
    IHttpContextAccessor httpContextAccessor,
    ILogger<GraphHttpClient> logger) 
    : IGraphClient
{
    private void PopulateToken()
    {
        string token = httpContextAccessor.HttpContext?.Request.Headers[IntelliPathHeaders.GraphTokenHeader].ToString() ?? string.Empty;
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
        catch(Exception e)
        {
            logger.LogWarning("Failed to get user information from Graph API. Error: {Error}", e.Message);
            Console.WriteLine(e);
        }
        return null;
    }
}

public interface IGraphClient
{
    Task<UserModel?> GetMeAsync(string select = "displayName,department,aboutMe,jobTitle");
}