using FlowForge.Net;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

var services = new ServiceCollection();
services.AddLogging(builder => builder.AddConsole());
services.AddJobBuilder();

var provider = services.BuildServiceProvider();

var jobBuilder = provider.GetRequiredService<JobBuilder>();

jobBuilder.ConfigureRepository(
    new NpgsqlJobStepRepository("Server=127.0.0.1;Port=5432;Database=test;User Id=test;Password=test;Include Error Detail=true"));
var job = jobBuilder
    .WithName("SampleJob")
    .Next(new SimpleStep("1"))
    .Next(new SimpleStep("2"))
    .Next(new SimpleStep("3"))
    .Next(new SimpleStep("4"))
    .Build();

await job.ExecuteAsync();

internal class SimpleStep : Step
{
    public SimpleStep()
    {
        Name = nameof(SimpleStep);
    }

    public SimpleStep(string stepName)
    {
        Name = stepName;
    }
    
    public new async Task<StepResult> ExecuteAsync()
    {
        var number = Random.Shared.Next(1, 10);

        await Task.Delay(number * 500);

        StepResult = number switch
        {
            < 2 => StepResult.Failed("Failed", new ApplicationException("Failed")),
            <= 8 => StepResult.Success(),
            _ => StepResult.Invalid("Invalid")
        };

        return StepResult;
    }
}