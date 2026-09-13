using System.Text.Json.Serialization;

namespace StravaMCP.Strava.Models;

public sealed record ActivityStats
{
    [JsonPropertyName("biggest_ride_distance")]
    public double? BiggestRideDistance { get; init; }

    [JsonPropertyName("biggest_climb_elevation_gain")]
    public double? BiggestClimbElevationGain { get; init; }

    [JsonPropertyName("recent_run_totals")]
    public required ActivityTotal RecentRunTotals { get; init; }

    [JsonPropertyName("recent_ride_totals")]
    public required ActivityTotal RecentRideTotals { get; init; }

    [JsonPropertyName("recent_swim_totals")]
    public required ActivityTotal RecentSwimTotals { get; init; }

    [JsonPropertyName("ytd_run_totals")]
    public required ActivityTotal YtdRunTotals { get; init; }

    [JsonPropertyName("ytd_ride_totals")]
    public required ActivityTotal YtdRideTotals { get; init; }

    [JsonPropertyName("ytd_swim_totals")]
    public required ActivityTotal YtdSwimTotals { get; init; }

    [JsonPropertyName("all_run_totals")]
    public required ActivityTotal AllRunTotals { get; init; }

    [JsonPropertyName("all_ride_totals")]
    public required ActivityTotal AllRideTotals { get; init; }

    [JsonPropertyName("all_swim_totals")]
    public required ActivityTotal AllSwimTotals { get; init; }
}
