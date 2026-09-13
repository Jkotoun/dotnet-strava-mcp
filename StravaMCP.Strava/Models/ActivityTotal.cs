using System.Text.Json.Serialization;

namespace StravaMCP.Strava.Models;

public sealed record ActivityTotal
{
    // Count/time/achievement fields are declared as double, not int: Strava's real stats
    // response emits at least some of these as JSON numbers with a decimal point (e.g. "123.0"),
    // which System.Text.Json's strict Int32 converter rejects outright.
    [JsonPropertyName("count")]
    public double Count { get; init; }

    [JsonPropertyName("distance")]
    public double Distance { get; init; }

    [JsonPropertyName("moving_time")]
    public double MovingTime { get; init; }

    [JsonPropertyName("elapsed_time")]
    public double ElapsedTime { get; init; }

    [JsonPropertyName("elevation_gain")]
    public double ElevationGain { get; init; }

    [JsonPropertyName("achievement_count")]
    public double AchievementCount { get; init; }
}
