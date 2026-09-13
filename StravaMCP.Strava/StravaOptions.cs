using System.ComponentModel.DataAnnotations;

namespace StravaMCP.Strava;

public sealed class StravaOptions
{
    [Required]
    public required string ClientId { get; init; }

    [Required]
    public required string ClientSecret { get; init; }

    [Required]
    public required string RefreshToken { get; init; }

    [Required]
    public required string ApiBaseUrl { get; init; }

    [Required]
    public required string TokenUrl { get; init; }
}
