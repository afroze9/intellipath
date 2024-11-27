namespace IntelliPath.Orchestrator.Configuration;

public class IdentityOptions
{
    public required string ClientId { get; set; }

    public required string ClientSecret { get; set; }

    public required string TenantId { get; set; }

    public required string[] Scopes { get; set; } = [];
}