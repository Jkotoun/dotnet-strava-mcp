using System.ComponentModel;
using ModelContextProtocol.Server;
using StravaMCP.Strava;
using StravaMCP.Strava.Models;

namespace StravaMCP.Server.SdkVariant.Tools;

[McpServerToolType]
public sealed class StravaTools(StravaClient stravaClient)
{
    [McpServerTool, Description("Gets the authenticated Strava athlete's profile.")]
    public Task<AthleteProfile> GetAthleteProfile(CancellationToken cancellationToken) =>
        stravaClient.GetAthleteProfileAsync(cancellationToken);

    [McpServerTool, Description("Gets the authenticated athlete's most recent activities.")]
    public Task<IReadOnlyList<SummaryActivity>> GetRecentActivities(
        [Description("Maximum number of activities to return.")] int count = 10,
        CancellationToken cancellationToken = default) =>
        stravaClient.GetRecentActivitiesAsync(count, cancellationToken);

    [McpServerTool, Description("Gets detailed information for a single Strava activity.")]
    public Task<DetailedActivity> GetActivityDetail(
        [Description("The Strava activity ID.")] long activityId,
        CancellationToken cancellationToken = default) =>
        stravaClient.GetActivityDetailAsync(activityId, cancellationToken);

    [McpServerTool, Description("Gets the authenticated athlete's activity totals (recent, year-to-date, all-time) for runs, rides, and swims.")]
    public Task<ActivityStats> GetAthleteStats(CancellationToken cancellationToken = default) =>
        stravaClient.GetAuthenticatedAthleteStatsAsync(cancellationToken);
}
