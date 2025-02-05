using FlowForge.Net;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

var host = new HostBuilder()
    .UseEnvironment(Environments.Development)
    .ConfigureAppConfiguration((context, builder) =>
    {
        builder.AddJsonFile("appsettings.json");
        builder.AddJsonFile($"appsettings.{context.HostingEnvironment.EnvironmentName}.json");
    })
    .ConfigureServices((context, services) =>
    {
        services.AddServiceDiscovery();
        services.AddJobBuilder(context.Configuration.GetConnectionString("postgresdb")!);
    })
    .ConfigureLogging(config =>
    {
        config.AddConsole();
    })
    .Build();

await host.StartAsync();

var jobBuilder = host.Services.GetRequiredService<JobBuilder>();

var job = jobBuilder
    .WithName("SampleJob")
    .Next(new SimpleStep("1"))
    .Next(new SimpleStep("2"))
    .Next(new SimpleStep("3"))
    .Next(new SimpleStep("4"))
    .Build();

await job.ExecuteAsync();

await host.StopAsync();