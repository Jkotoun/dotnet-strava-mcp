using System.Net.Http.Json;
using Microsoft.Extensions.Options;

namespace StravaMCP.Strava;

public sealed class StravaClient
{
    private readonly HttpClient _httpClient;

    public StravaClient(HttpClient httpClient, IOptions<StravaOptions> options)
    {
        _httpClient = httpClient;
        _httpClient.BaseAddress = new Uri(options.Value.ApiBaseUrl);
    }

    public async Task<AthleteProfile> GetAthleteProfileAsync(CancellationToken ct) =>
        await _httpClient.GetFromJsonAsync<AthleteProfile>("athlete", ct)
            ?? throw new InvalidOperationException("Strava returned an empty athlete profile.");
}
