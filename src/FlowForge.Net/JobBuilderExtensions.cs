using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace FlowForge.Net;

public static class JobBuilderExtensions
{
    public static IServiceCollection AddJobBuilder(this IServiceCollection services, string connectionString)
    {
        ArgumentNullException.ThrowIfNull(services);

        services.AddSingleton<JobBuilder>();
        services.AddSingleton(provider =>
        {
            var jobBuilder = provider.GetRequiredService<JobBuilder>();
            jobBuilder.UseNpgsql(connectionString);
            return jobBuilder;
        });
        
        return services;
    }
}
