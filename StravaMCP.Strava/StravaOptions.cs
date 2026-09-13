namespace StravaMCP.Strava;

public sealed class StravaOptions
{
    public required string ClientId { get; init; }
    public required string ClientSecret { get; init; }
    public required string RefreshToken { get; init; }
    public required string ApiBaseUrl { get; init; }
    public required string TokenUrl { get; init; }
}
