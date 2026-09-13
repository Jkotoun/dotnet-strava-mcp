using System.Net;
using System.Text;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using StravaMCP.Strava;

namespace StravaMCP.Tests.Common;

/// <summary>
/// A WebApplicationFactory that fakes the Strava HTTP boundary (token endpoint + API endpoint) with
/// canned responses, so tests exercise the real dispatcher/tool/StravaClient/StravaAuthClient code
/// end-to-end without ever calling the live Strava API.
/// </summary>
public sealed class StravaFakeWebApplicationFactory<TProgram> : WebApplicationFactory<TProgram>
    where TProgram : class
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            services.AddHttpClient("StravaAuth")
                .ConfigurePrimaryHttpMessageHandler(() =>
                    new StubHttpMessageHandler(_ => JsonResponse(StravaFixtures.TokenResponseJson)));

            services.AddHttpClient<StravaClient>()
                .ConfigurePrimaryHttpMessageHandler(() =>
                    new StubHttpMessageHandler(request => JsonResponse(StravaFixtures.ResponseFor(request.RequestUri!))));
        });
    }

    private static HttpResponseMessage JsonResponse(string json) => new(HttpStatusCode.OK)
    {
        Content = new StringContent(json, Encoding.UTF8, "application/json"),
    };
}
