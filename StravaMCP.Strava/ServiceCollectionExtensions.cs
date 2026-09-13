using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace StravaMCP.Strava;

public static class ServiceCollectionExtensions
{
    /// <summary>Registers the Strava OAuth2/API client stack shared by both server variants.</summary>
    public static IServiceCollection AddStravaApi(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<StravaOptions>(configuration.GetSection("Strava"));
        services.AddHttpClient("StravaAuth");
        services.AddSingleton<StravaAuthClient>();
        services.AddTransient<StravaAuthHandler>();
        services
            .AddHttpClient<StravaClient>()
            .AddHttpMessageHandler<StravaAuthHandler>();

        return services;
    }
}
