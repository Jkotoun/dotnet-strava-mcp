using System.Text.Json;
using StravaMCP.Server.FromScratchVariant.Mcp;
using StravaMCP.Strava;

namespace StravaMCP.Server.FromScratchVariant.Tools;

public sealed class GetAthleteStatsTool(StravaClient stravaClient) : IMcpTool
{
    public string Name => "get_athlete_stats";

    public string Description =>
        "Gets the authenticated athlete's activity totals (recent, year-to-date, all-time) for runs, rides, and swims.";

    public JsonElement InputSchema { get; } = JsonDocument.Parse("""{"type":"object","properties":{}}""").RootElement;

    public async Task<object?> ExecuteAsync(JsonElement arguments, CancellationToken ct) =>
        await stravaClient.GetAuthenticatedAthleteStatsAsync(ct);
}
