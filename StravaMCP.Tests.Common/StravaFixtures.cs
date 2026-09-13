namespace StravaMCP.Tests.Common;

/// <summary>Canned Strava API response bodies for tests that fake the HTTP boundary instead of calling Strava for real.</summary>
public static class StravaFixtures
{
    public const string TokenResponseJson = """
        {"access_token":"fake-access-token","refresh_token":"fake-refresh-token","expires_at":9999999999}
        """;

    public const string AthleteProfileJson = """
        {"id":1,"username":"fake_athlete","firstname":"Fake","lastname":"Athlete","city":"Testville","country":"Testland"}
        """;

    public const string RecentActivitiesJson = """
        [{"id":100,"name":"Fake Ride","distance":10000,"moving_time":1800,"elapsed_time":1900,"total_elevation_gain":50,"type":"Ride","sport_type":"Ride","start_date":"2026-01-01T08:00:00Z","average_speed":5.5,"max_speed":9.0,"average_heartrate":140,"kudos_count":2}]
        """;

    public const string ActivityDetailJson = """
        {"id":100,"name":"Fake Ride","description":"A fake ride for tests","distance":10000,"moving_time":1800,"elapsed_time":1900,"total_elevation_gain":50,"type":"Ride","sport_type":"Ride","start_date":"2026-01-01T08:00:00Z","average_speed":5.5,"max_speed":9.0,"average_heartrate":140,"max_heartrate":160,"calories":300,"kudos_count":2}
        """;

    public const string AthleteStatsJson = """
        {"biggest_ride_distance":50000,"biggest_climb_elevation_gain":500,"recent_run_totals":{"count":1,"distance":5000,"moving_time":1500,"elapsed_time":1600,"elevation_gain":20,"achievement_count":0},"recent_ride_totals":{"count":1,"distance":10000,"moving_time":1800,"elapsed_time":1900,"elevation_gain":50,"achievement_count":0},"recent_swim_totals":{"count":0,"distance":0,"moving_time":0,"elapsed_time":0,"elevation_gain":0,"achievement_count":0},"ytd_run_totals":{"count":10,"distance":50000,"moving_time":15000,"elapsed_time":16000,"elevation_gain":200,"achievement_count":0},"ytd_ride_totals":{"count":10,"distance":100000,"moving_time":18000,"elapsed_time":19000,"elevation_gain":500,"achievement_count":0},"ytd_swim_totals":{"count":0,"distance":0,"moving_time":0,"elapsed_time":0,"elevation_gain":0,"achievement_count":0},"all_run_totals":{"count":20,"distance":100000,"moving_time":30000,"elapsed_time":32000,"elevation_gain":400,"achievement_count":0},"all_ride_totals":{"count":20,"distance":200000,"moving_time":36000,"elapsed_time":38000,"elevation_gain":1000,"achievement_count":0},"all_swim_totals":{"count":0,"distance":0,"moving_time":0,"elapsed_time":0,"elevation_gain":0,"achievement_count":0}}
        """;

    /// <summary>Routes a fake Strava API request to the matching canned response by path.</summary>
    public static string ResponseFor(Uri requestUri)
    {
        var path = requestUri.AbsolutePath;

        if (path.EndsWith("/athlete/activities", StringComparison.Ordinal))
        {
            return RecentActivitiesJson;
        }

        if (path.EndsWith("/athlete", StringComparison.Ordinal))
        {
            return AthleteProfileJson;
        }

        if (path.Contains("/activities/", StringComparison.Ordinal))
        {
            return ActivityDetailJson;
        }

        if (path.Contains("/athletes/", StringComparison.Ordinal) && path.EndsWith("/stats", StringComparison.Ordinal))
        {
            return AthleteStatsJson;
        }

        throw new InvalidOperationException($"No Strava fixture configured for request path '{path}'.");
    }
}
