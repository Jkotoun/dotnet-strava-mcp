using System.Text.Json.Serialization;

namespace StravaMCP.Strava.Models;

public sealed record StravaTokenResponse
{
    [JsonPropertyName("access_token")]
    public required string AccessToken { get; init; }

    [JsonPropertyName("refresh_token")]
    public required string RefreshToken { get; init; }

    [JsonPropertyName("expires_at")]
    public required long ExpiresAt { get; init; }
}
