using System.Net;
using System.Net.Http.Json;
using Microsoft.Extensions.Options;
using StravaMCP.Tests.Common;
using Xunit;

namespace StravaMCP.Strava.Tests;

public class StravaAuthClientTests
{
    private static StravaOptions CreateOptions() => new()
    {
        ClientId = "client-id",
        ClientSecret = "client-secret",
        RefreshToken = "refresh-token",
        ApiBaseUrl = "https://www.strava.com/api/v3/",
        TokenUrl = "https://www.strava.com/oauth/token",
    };

    private static StravaAuthClient CreateSut(StubHttpMessageHandler handler) =>
        new(new StubHttpClientFactory(new HttpClient(handler)), Options.Create(CreateOptions()));

    private static HttpResponseMessage TokenResponse(string accessToken, long expiresAt) => new(HttpStatusCode.OK)
    {
        Content = JsonContent.Create(new
        {
            access_token = accessToken,
            refresh_token = "rotated-refresh-token",
            expires_at = expiresAt,
        }),
    };

    [Fact]
    public async Task GetAccessTokenAsync_CachesTokenAcrossCalls()
    {
        var farFuture = DateTimeOffset.UtcNow.AddHours(1).ToUnixTimeSeconds();
        var handler = new StubHttpMessageHandler(_ => TokenResponse("token-a", farFuture));
        var sut = CreateSut(handler);

        var first = await sut.GetAccessTokenAsync(CancellationToken.None);
        var second = await sut.GetAccessTokenAsync(CancellationToken.None);

        Assert.Equal("token-a", first);
        Assert.Equal("token-a", second);
        Assert.Equal(1, handler.CallCount);
    }

    [Fact]
    public async Task GetAccessTokenAsync_RefreshesOnceCachedTokenIsPastTheExpiryBuffer()
    {
        // 30s out - inside the 1-minute safety buffer StravaAuthClient applies, so already stale.
        var almostExpired = DateTimeOffset.UtcNow.AddSeconds(30).ToUnixTimeSeconds();
        var farFuture = DateTimeOffset.UtcNow.AddHours(1).ToUnixTimeSeconds();
        var responses = new Queue<HttpResponseMessage>(
        [
            TokenResponse("token-a", almostExpired),
            TokenResponse("token-b", farFuture),
        ]);
        var handler = new StubHttpMessageHandler(_ => responses.Dequeue());
        var sut = CreateSut(handler);

        var first = await sut.GetAccessTokenAsync(CancellationToken.None);
        var second = await sut.GetAccessTokenAsync(CancellationToken.None);

        Assert.Equal("token-a", first);
        Assert.Equal("token-b", second);
        Assert.Equal(2, handler.CallCount);
    }

    [Fact]
    public async Task GetAccessTokenAsync_SendsRefreshTokenGrant()
    {
        var farFuture = DateTimeOffset.UtcNow.AddHours(1).ToUnixTimeSeconds();
        var handler = new StubHttpMessageHandler(_ => TokenResponse("token-a", farFuture));
        var sut = CreateSut(handler);

        await sut.GetAccessTokenAsync(CancellationToken.None);

        Assert.Equal(HttpMethod.Post, handler.LastRequest!.Method);
        Assert.Equal("https://www.strava.com/oauth/token", handler.LastRequest.RequestUri!.ToString());
        Assert.Contains("grant_type=refresh_token", handler.LastRequestBody);
        Assert.Contains("refresh_token=refresh-token", handler.LastRequestBody);
    }
}
