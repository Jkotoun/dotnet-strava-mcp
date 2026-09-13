using StravaMCP.Strava;

namespace StravaMCP.Tests.Common;

/// <summary>Canned StravaOptions for tests that don't care about the actual credential values.</summary>
public static class StravaOptionsFixture
{
    public static StravaOptions Create() => new()
    {
        ClientId = "client-id",
        ClientSecret = "client-secret",
        RefreshToken = "refresh-token",
        ApiBaseUrl = "https://www.strava.com/api/v3/",
        TokenUrl = "https://www.strava.com/oauth/token",
    };
}
