using Microsoft.Extensions.DependencyInjection;

namespace FlowForge.Net;

public static class JobBuilderExtensions
{
    public static IServiceCollection AddJobBuilder(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        services.AddSingleton<JobBuilder>();
        return services;
    }
}
