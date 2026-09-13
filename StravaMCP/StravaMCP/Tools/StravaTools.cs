using System.ComponentModel;
using ModelContextProtocol.Server;
using StravaMCP.Strava;

namespace StravaMCP.Tools;

[McpServerToolType]
public sealed class StravaTools(StravaClient stravaClient)
{
    [McpServerTool, Description("Gets the authenticated Strava athlete's profile.")]
    public Task<AthleteProfile> GetAthleteProfile(CancellationToken cancellationToken) =>
        stravaClient.GetAthleteProfileAsync(cancellationToken);
}
