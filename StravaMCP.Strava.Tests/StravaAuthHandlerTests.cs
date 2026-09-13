using System.Net;
using System.Net.Http.Json;
using Microsoft.Extensions.Options;
using StravaMCP.Tests.Common;
using Xunit;

namespace StravaMCP.Strava.Tests;

public class StravaAuthHandlerTests
{
    [Fact]
    public async Task SendAsync_AttachesBearerTokenFromAuthClient()
    {
        var tokenHandler = new StubHttpMessageHandler(_ => new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = JsonContent.Create(new
            {
                access_token = "fetched-access-token",
                refresh_token = "rotated-refresh-token",
                expires_at = DateTimeOffset.UtcNow.AddHours(1).ToUnixTimeSeconds(),
            }),
        });
        var authClient = new StravaAuthClient(
            new StubHttpClientFactory(new HttpClient(tokenHandler)),
            Options.Create(StravaOptionsFixture.Create()));

        var innerHandler = new StubHttpMessageHandler(_ => new HttpResponseMessage(HttpStatusCode.OK));
        var sut = new StravaAuthHandler(authClient) { InnerHandler = innerHandler };
        using var client = new HttpClient(sut) { BaseAddress = new Uri("https://www.strava.com/api/v3/") };

        await client.GetAsync("athlete");

        Assert.Equal("Bearer", innerHandler.LastRequest!.Headers.Authorization!.Scheme);
        Assert.Equal("fetched-access-token", innerHandler.LastRequest.Headers.Authorization.Parameter);
    }
}
