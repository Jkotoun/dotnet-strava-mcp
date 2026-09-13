using System.Net.Http.Json;
using Microsoft.Extensions.Options;
using StravaMCP.Strava.Models;

namespace StravaMCP.Strava;

/// <summary>
/// Registered as a singleton (not via AddHttpClient&lt;T&gt;, which would make it transient) so the
/// cached access token actually survives across requests instead of being reset on every injection.
/// </summary>
public sealed class StravaAuthClient(IHttpClientFactory httpClientFactory, IOptions<StravaOptions> options) : IDisposable
{
    private readonly HttpClient _httpClient = httpClientFactory.CreateClient("StravaAuth");
    private readonly StravaOptions _options = options.Value;
    private readonly SemaphoreSlim _refreshLock = new(1, 1);

    private string? _cachedAccessToken;
    private DateTimeOffset _cachedAccessTokenExpiresAt = DateTimeOffset.MinValue;

    public async Task<string> GetAccessTokenAsync(CancellationToken ct)
    {
        if (IsCachedTokenValid())
        {
            return _cachedAccessToken!;
        }

        await _refreshLock.WaitAsync(ct);
        try
        {
            // Re-check: another caller may have already refreshed while we were waiting for the lock.
            if (IsCachedTokenValid())
            {
                return _cachedAccessToken!;
            }

            using var response = await _httpClient.PostAsync(
                _options.TokenUrl,
                new FormUrlEncodedContent(new Dictionary<string, string>
                {
                    ["client_id"] = _options.ClientId,
                    ["client_secret"] = _options.ClientSecret,
                    ["refresh_token"] = _options.RefreshToken,
                    ["grant_type"] = "refresh_token",
                }),
                ct);

            response.EnsureSuccessStatusCode();

            var token = await response.Content.ReadFromJsonAsync<StravaTokenResponse>(cancellationToken: ct)
                ?? throw new InvalidOperationException("Strava token endpoint returned an empty response.");

            _cachedAccessToken = token.AccessToken;
            _cachedAccessTokenExpiresAt = DateTimeOffset.FromUnixTimeSeconds(token.ExpiresAt);

            return _cachedAccessToken;
        }
        finally
        {
            _refreshLock.Release();
        }
    }

    // One minute of slack so an in-flight request never gets handed a token that expires mid-call.
    private bool IsCachedTokenValid() =>
        _cachedAccessToken is not null && DateTimeOffset.UtcNow < _cachedAccessTokenExpiresAt - TimeSpan.FromMinutes(1);

    public void Dispose() => _refreshLock.Dispose();
}
