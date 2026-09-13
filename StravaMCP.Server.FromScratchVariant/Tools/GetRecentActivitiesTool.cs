using System.Text.Json;
using StravaMCP.Server.FromScratchVariant.Mcp;
using StravaMCP.Strava;

namespace StravaMCP.Server.FromScratchVariant.Tools;

public sealed class GetRecentActivitiesTool(StravaClient stravaClient) : IMcpTool
{
    public string Name => "get_recent_activities";
    public string Description => "Gets the authenticated athlete's most recent activities.";

    public JsonElement InputSchema { get; } = JsonDocument.Parse("""
        {"type":"object","properties":{"count":{"type":"integer","description":"Maximum number of activities to return.","default":10}}}
        """).RootElement;

    public async Task<object?> ExecuteAsync(JsonElement arguments, CancellationToken ct)
    {
        var count = arguments.ValueKind == JsonValueKind.Object && arguments.TryGetProperty("count", out var countElement)
            ? countElement.GetInt32()
            : 10;

        return await stravaClient.GetRecentActivitiesAsync(count, ct);
    }
}
