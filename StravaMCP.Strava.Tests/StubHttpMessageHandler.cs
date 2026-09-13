namespace StravaMCP.Strava.Tests;

/// <summary>Test double for HttpClient: no real network calls, just canned responses.</summary>
internal sealed class StubHttpMessageHandler(Func<HttpRequestMessage, HttpResponseMessage> responder) : HttpMessageHandler
{
    public int CallCount { get; private set; }
    public HttpRequestMessage? LastRequest { get; private set; }
    public string? LastRequestBody { get; private set; }

    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        CallCount++;
        LastRequest = request;
        LastRequestBody = request.Content is null
            ? null
            : await request.Content.ReadAsStringAsync(cancellationToken);

        return responder(request);
    }
}

internal sealed class StubHttpClientFactory(HttpClient client) : IHttpClientFactory
{
    public HttpClient CreateClient(string name) => client;
}
