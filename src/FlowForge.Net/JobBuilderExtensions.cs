using Microsoft.Extensions.DependencyInjection;

namespace FlowForge.Net;

/// <inheritdoc />
public static class JobBuilderExtensions
{
    /// <inheritdoc />
    public static IServiceCollection AddJobBuilder(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        services.AddSingleton<JobBuilder>();
        return services;
    }
    
    /// <inheritdoc />
    public static IServiceCollection AddJobBuilder(this IServiceCollection services, Action<JobBuilder> configure)
    {
        ArgumentNullException.ThrowIfNull(services);

        services.AddSingleton<JobBuilder>();
        return services;
    }
}