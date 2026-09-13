using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace StravaMCP.Strava;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddStravaApi(this IServiceCollection services, IConfiguration configuration)
    {
        services
            .AddOptions<StravaOptions>()
            .Bind(configuration.GetSection("Strava"))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services.AddHttpClient("StravaAuth");
        services.AddSingleton<StravaAuthClient>();
        services.AddTransient<StravaAuthHandler>();
        services
            .AddHttpClient<StravaClient>((sp, client) =>
                client.BaseAddress = new Uri(sp.GetRequiredService<IOptions<StravaOptions>>().Value.ApiBaseUrl))
            .AddHttpMessageHandler<StravaAuthHandler>();

        return services;
    }
}
