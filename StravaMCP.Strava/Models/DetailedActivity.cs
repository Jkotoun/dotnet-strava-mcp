using System.Text.Json.Serialization;

namespace StravaMCP.Strava.Models;

public sealed record DetailedActivity
{
    [JsonPropertyName("id")]
    public required long Id { get; init; }

    [JsonPropertyName("name")]
    public string? Name { get; init; }

    [JsonPropertyName("description")]
    public string? Description { get; init; }

    [JsonPropertyName("distance")]
    public double Distance { get; init; }

    [JsonPropertyName("moving_time")]
    public int MovingTime { get; init; }

    [JsonPropertyName("elapsed_time")]
    public int ElapsedTime { get; init; }

    [JsonPropertyName("total_elevation_gain")]
    public double TotalElevationGain { get; init; }

    [JsonPropertyName("type")]
    public string? Type { get; init; }

    [JsonPropertyName("sport_type")]
    public string? SportType { get; init; }

    [JsonPropertyName("start_date")]
    public DateTimeOffset StartDate { get; init; }

    [JsonPropertyName("average_speed")]
    public double AverageSpeed { get; init; }

    [JsonPropertyName("max_speed")]
    public double MaxSpeed { get; init; }

    [JsonPropertyName("average_heartrate")]
    public double? AverageHeartrate { get; init; }

    [JsonPropertyName("max_heartrate")]
    public double? MaxHeartrate { get; init; }

    [JsonPropertyName("calories")]
    public double? Calories { get; init; }

    [JsonPropertyName("kudos_count")]
    public int KudosCount { get; init; }
}
