using System.Text.Json.Serialization;

namespace StravaMCP.Strava.Models;

public sealed record ActivityTotal
{
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
