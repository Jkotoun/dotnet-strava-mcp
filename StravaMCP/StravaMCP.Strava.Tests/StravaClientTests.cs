using System.Net;
using System.Net.Http.Json;
using Microsoft.Extensions.Options;
using Xunit;

namespace StravaMCP.Strava.Tests;

public class StravaClientTests
{
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
        var options = Options.Create(new StravaOptions
        {
            ClientId = "id",
            ClientSecret = "secret",
            RefreshToken = "refresh",
            ApiBaseUrl = "https://www.strava.com/api/v3/",
            TokenUrl = "https://www.strava.com/oauth/token",
        });
        var sut = new StravaClient(new HttpClient(handler), options);

        var profile = await sut.GetAthleteProfileAsync(CancellationToken.None);

        // Regression guard: HttpClient.BaseAddress + a relative path combine per standard URI-reference
        // rules - a base URL missing its trailing slash silently drops the last path segment ("v3").
        Assert.Equal("https://www.strava.com/api/v3/athlete", handler.LastRequest!.RequestUri!.ToString());
        Assert.Equal(52596209, profile.Id);
        Assert.Equal("josef_kotoun", profile.Username);
        Assert.Equal("Josef", profile.FirstName);
        Assert.Equal("Kotoun", profile.LastName);
    }
}
