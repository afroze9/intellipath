using Azure.Core;
using Microsoft.Graph;

namespace IntelliPath.Orchestrator.Services;

public class GraphClientFactory(IHttpContextAccessor httpContextAccessor) : IGraphClientFactory
{
    public GraphServiceClient Create()
    {
        string headerToken = httpContextAccessor?.HttpContext?.Request.Headers["x-MS-Graph-Token"].ToString() ?? string.Empty;
        TokenCredential tokenCredential = DelegatedTokenCredential.Create(
            getToken: (_, _) => new AccessToken(headerToken, DateTimeOffset.Now.AddMinutes(3)),
            getTokenAsync: (_, _) => ValueTask.FromResult(new AccessToken(headerToken, DateTimeOffset.Now.AddMinutes(3)))
        );
        
        return new GraphServiceClient(tokenCredential);
    }
}

public interface IGraphClientFactory
{
    GraphServiceClient Create();
}