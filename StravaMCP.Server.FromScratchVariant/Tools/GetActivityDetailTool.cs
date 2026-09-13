using System.Text.Json;
using StravaMCP.Server.FromScratchVariant.Mcp;
using StravaMCP.Strava;

namespace StravaMCP.Server.FromScratchVariant.Tools;

public sealed class GetActivityDetailTool(StravaClient stravaClient) : IMcpTool
{
    public string Name => "get_activity_detail";
    public string Description => "Gets detailed information for a single Strava activity.";

    public JsonElement InputSchema { get; } = JsonDocument.Parse("""
        {"type":"object","properties":{"activityId":{"type":"integer","description":"The Strava activity ID."}},"required":["activityId"]}
        """).RootElement;

    public async Task<object?> ExecuteAsync(JsonElement arguments, CancellationToken ct)
    {
        if (arguments.ValueKind != JsonValueKind.Object || !arguments.TryGetProperty("activityId", out var activityIdElement))
        {
            throw new ArgumentException("Missing required argument 'activityId'.");
        }

        return await stravaClient.GetActivityDetailAsync(activityIdElement.GetInt64(), ct);
    }
}
