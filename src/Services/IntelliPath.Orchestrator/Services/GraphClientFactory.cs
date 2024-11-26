using IntelliPath.Shared.Models;
using Microsoft.Graph;
using Microsoft.Identity.Client;
using Microsoft.Kiota.Abstractions.Authentication;

namespace IntelliPath.Orchestrator.Services;

public class GraphClientFactory(IHttpContextAccessor httpContextAccessor) : IGraphClientFactory
{
    public GraphServiceClient Create()
    {
        TokenProvider tokenProvider = new TokenProvider(httpContextAccessor);
        BaseBearerTokenAuthenticationProvider accessTokenProvider = new BaseBearerTokenAuthenticationProvider(tokenProvider);
        return new GraphServiceClient(accessTokenProvider);
    }
}

public interface IGraphClientFactory
{
    GraphServiceClient Create();
}

public class TokenProvider(IHttpContextAccessor httpContextAccessor) : IAccessTokenProvider
{
    public async Task<string> GetAuthorizationTokenAsync(Uri uri, Dictionary<string, object>? additionalAuthenticationContext = null,
        CancellationToken cancellationToken = new CancellationToken())
    {
        string headerToken = httpContextAccessor?.HttpContext?.Request.Headers[IntelliPathHeaders.GraphTokenHeader].ToString() ?? string.Empty;
        IConfidentialClientApplication app = ConfidentialClientApplicationBuilder
            .Create("")
            .WithClientSecret("")
            .WithAuthority(new Uri(""))
            .Build();
        UserAssertion userAssertion = new UserAssertion(headerToken);
        AuthenticationResult result = await app.AcquireTokenOnBehalfOf([""], userAssertion).ExecuteAsync(cancellationToken);
        return result.AccessToken;
    }

    public AllowedHostsValidator AllowedHostsValidator { get; }
}