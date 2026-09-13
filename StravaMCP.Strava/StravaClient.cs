using System.Net.Http.Json;
using StravaMCP.Strava.Models;

namespace StravaMCP.Strava;

public sealed class StravaClient(HttpClient httpClient)
{
    private readonly HttpClient _httpClient = httpClient;

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

    /// <summary>Strava's stats endpoint needs an athlete id, which the authenticated athlete only learns from their own profile.</summary>
    public async Task<ActivityStats> GetAuthenticatedAthleteStatsAsync(CancellationToken ct)
    {
        var profile = await GetAthleteProfileAsync(ct);
        return await GetAthleteStatsAsync(profile.Id, ct);
    }
}
