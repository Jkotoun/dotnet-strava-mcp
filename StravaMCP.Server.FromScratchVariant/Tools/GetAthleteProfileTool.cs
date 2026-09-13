using System.Text.Json;
using StravaMCP.Server.FromScratchVariant.Mcp;
using StravaMCP.Strava;

namespace StravaMCP.Server.FromScratchVariant.Tools;

public sealed class GetAthleteProfileTool(StravaClient stravaClient) : IMcpTool
{
    public string Name => "get_athlete_profile";
    public string Description => "Gets the authenticated Strava athlete's profile.";

    public JsonElement InputSchema { get; } = JsonDocument.Parse("""{"type":"object","properties":{}}""").RootElement;

    public async Task<object?> ExecuteAsync(JsonElement arguments, CancellationToken ct) =>
        await stravaClient.GetAthleteProfileAsync(ct);
}
