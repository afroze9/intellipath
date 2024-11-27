using System.Net.Http.Headers;
using IntelliPath.Orchestrator.Configuration;
using IntelliPath.Shared.Models;
using Microsoft.Extensions.Options;
using Microsoft.Graph;
using Microsoft.Identity.Client;
using Microsoft.Kiota.Abstractions.Authentication;

namespace IntelliPath.Orchestrator.Services;

public class GraphClientFactory(IHttpContextAccessor httpContextAccessor, IOptions<IdentityOptions> identityOptions) : IGraphClientFactory
{
    public GraphServiceClient Create()
    {
        string headerToken = httpContextAccessor?.HttpContext?.Request.Headers[IntelliPathHeaders.GraphTokenHeader].ToString() ?? string.Empty;
        //TokenProvider tokenProvider = new TokenProvider(httpContextAccessor, identityOptions.Value);
        //BaseBearerTokenAuthenticationProvider accessTokenProvider = new BaseBearerTokenAuthenticationProvider(tokenProvider);
        HttpClient httpClient = new HttpClient();
        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", headerToken);
        var client = new GraphServiceClient(httpClient);
        return client;
        //return new GraphServiceClient(accessTokenProvider);
    }
}

public interface IGraphClientFactory
{
    GraphServiceClient Create();
}

public class TokenProvider(IHttpContextAccessor httpContextAccessor, IdentityOptions options) : IAccessTokenProvider
{
    public async Task<string> GetAuthorizationTokenAsync(Uri uri, Dictionary<string, object>? additionalAuthenticationContext = null,
        CancellationToken cancellationToken = new CancellationToken())
    {
        string headerToken = httpContextAccessor?.HttpContext?.Request.Headers[IntelliPathHeaders.GraphTokenHeader].ToString() ?? string.Empty;
        IConfidentialClientApplication app = ConfidentialClientApplicationBuilder
            .Create(options.ClientId)
            .WithClientSecret(options.ClientSecret)
            .WithAuthority(new Uri($"https://login.microsoftonline.com/{options.TenantId}"))
            .Build();
        UserAssertion userAssertion = new UserAssertion(headerToken);
        AuthenticationResult result = await app.AcquireTokenOnBehalfOf(options.Scopes, userAssertion).ExecuteAsync(cancellationToken);
        return result.AccessToken;
    }

    public AllowedHostsValidator AllowedHostsValidator { get; }
}