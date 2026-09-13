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

    public async Task<IReadOnlyList<SummaryActivity>> GetRecentActivitiesAsync(int count, CancellationToken ct) =>
        await _httpClient.GetFromJsonAsync<List<SummaryActivity>>($"athlete/activities?per_page={count}", ct)
            ?? throw new InvalidOperationException("Strava returned an empty activities list.");

    public async Task<DetailedActivity> GetActivityDetailAsync(long activityId, CancellationToken ct) =>
        await _httpClient.GetFromJsonAsync<DetailedActivity>($"activities/{activityId}", ct)
            ?? throw new InvalidOperationException("Strava returned an empty activity.");

    public async Task<ActivityStats> GetAthleteStatsAsync(long athleteId, CancellationToken ct) =>
        await _httpClient.GetFromJsonAsync<ActivityStats>($"athletes/{athleteId}/stats", ct)
            ?? throw new InvalidOperationException("Strava returned empty athlete stats.");
}
