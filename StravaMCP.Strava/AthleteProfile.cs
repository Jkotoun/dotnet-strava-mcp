using System.Text.Json.Serialization;

namespace StravaMCP.Strava;

public sealed class AthleteProfile
{
    [JsonPropertyName("id")]
    public required long Id { get; init; }

    [JsonPropertyName("username")]
    public string? Username { get; init; }

    [JsonPropertyName("firstname")]
    public string? FirstName { get; init; }

    [JsonPropertyName("lastname")]
    public string? LastName { get; init; }

    [JsonPropertyName("city")]
    public string? City { get; init; }

    [JsonPropertyName("country")]
    public string? Country { get; init; }
}
