using System.Net;
using System.Net.Http.Json;
using Microsoft.Extensions.Options;
using StravaMCP.Tests.Common;
using Xunit;

namespace StravaMCP.Strava.Tests;

public class StravaClientTests
{
    private static StravaClient CreateSut(StubHttpMessageHandler handler)
    {
        var options = Options.Create(new StravaOptions
        {
            ClientId = "id",
            ClientSecret = "secret",
            RefreshToken = "refresh",
            ApiBaseUrl = "https://www.strava.com/api/v3/",
            TokenUrl = "https://www.strava.com/oauth/token",
        });
        return new StravaClient(new HttpClient(handler), options);
    }

    [Fact]
    public async Task GetAthleteProfileAsync_RequestsTheCorrectUrl_AndDeserializesTheResponse()
    {
        var handler = new StubHttpMessageHandler(_ => new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = JsonContent.Create(new
            {
                id = 52596209,
                username = "josef_kotoun",
                firstname = "Josef",
                lastname = "Kotoun",
                city = "",
                country = (string?)null,
            }),
        });
        var sut = CreateSut(handler);

        var profile = await sut.GetAthleteProfileAsync(CancellationToken.None);

        // Regression guard: HttpClient.BaseAddress + a relative path combine per standard URI-reference
        // rules - a base URL missing its trailing slash silently drops the last path segment ("v3").
        Assert.Equal("https://www.strava.com/api/v3/athlete", handler.LastRequest!.RequestUri!.ToString());
        Assert.Equal(52596209, profile.Id);
        Assert.Equal("josef_kotoun", profile.Username);
        Assert.Equal("Josef", profile.FirstName);
        Assert.Equal("Kotoun", profile.LastName);
    }

    [Fact]
    public async Task GetRecentActivitiesAsync_RequestsTheCorrectUrl_AndDeserializesTheResponse()
    {
        var handler = new StubHttpMessageHandler(_ => new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = JsonContent.Create(new[]
            {
                new
                {
                    id = 123L,
                    name = "Morning Run",
                    distance = 5000.0,
                    moving_time = 1800,
                    elapsed_time = 1900,
                    total_elevation_gain = 42.0,
                    type = "Run",
                    sport_type = "Run",
                    start_date = "2026-09-01T06:00:00Z",
                    average_speed = 2.78,
                    max_speed = 3.5,
                    average_heartrate = 150.0,
                    kudos_count = 3,
                },
            }),
        });
        var sut = CreateSut(handler);

        var activities = await sut.GetRecentActivitiesAsync(10, CancellationToken.None);

        Assert.Equal(
            "https://www.strava.com/api/v3/athlete/activities?per_page=10",
            handler.LastRequest!.RequestUri!.ToString());
        var activity = Assert.Single(activities);
        Assert.Equal(123L, activity.Id);
        Assert.Equal("Morning Run", activity.Name);
        Assert.Equal(5000.0, activity.Distance);
    }

    [Fact]
    public async Task GetActivityDetailAsync_RequestsTheCorrectUrl_AndDeserializesTheResponse()
    {
        var handler = new StubHttpMessageHandler(_ => new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = JsonContent.Create(new
            {
                id = 123L,
                name = "Morning Run",
                description = "Easy pace",
                distance = 5000.0,
                moving_time = 1800,
                elapsed_time = 1900,
                total_elevation_gain = 42.0,
                type = "Run",
                sport_type = "Run",
                start_date = "2026-09-01T06:00:00Z",
                average_speed = 2.78,
                max_speed = 3.5,
                average_heartrate = 150.0,
                max_heartrate = 172.0,
                calories = 450.0,
                kudos_count = 3,
            }),
        });
        var sut = CreateSut(handler);

        var activity = await sut.GetActivityDetailAsync(123L, CancellationToken.None);

        Assert.Equal("https://www.strava.com/api/v3/activities/123", handler.LastRequest!.RequestUri!.ToString());
        Assert.Equal(123L, activity.Id);
        Assert.Equal("Easy pace", activity.Description);
        Assert.Equal(450.0, activity.Calories);
    }

    [Fact]
    public async Task GetAthleteStatsAsync_RequestsTheCorrectUrl_AndDeserializesTheResponse()
    {
        var total = new
        {
            count = 10,
            distance = 50000.0,
            moving_time = 18000,
            elapsed_time = 19000,
            elevation_gain = 500.0,
            achievement_count = 2,
        };
        var handler = new StubHttpMessageHandler(_ => new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = JsonContent.Create(new
            {
                biggest_ride_distance = 100000.0,
                biggest_climb_elevation_gain = 800.0,
                recent_run_totals = total,
                recent_ride_totals = total,
                recent_swim_totals = total,
                ytd_run_totals = total,
                ytd_ride_totals = total,
                ytd_swim_totals = total,
                all_run_totals = total,
                all_ride_totals = total,
                all_swim_totals = total,
            }),
        });
        var sut = CreateSut(handler);

        var stats = await sut.GetAthleteStatsAsync(52596209L, CancellationToken.None);

        Assert.Equal(
            "https://www.strava.com/api/v3/athletes/52596209/stats",
            handler.LastRequest!.RequestUri!.ToString());
        Assert.Equal(100000.0, stats.BiggestRideDistance);
        Assert.Equal(10, stats.RecentRunTotals.Count);
        Assert.Equal(50000.0, stats.AllRideTotals.Distance);
    }
}
