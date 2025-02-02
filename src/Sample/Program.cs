using FlowForge.Net;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

var services = new ServiceCollection();
services.AddLogging(builder => builder.AddConsole());
services.AddJobBuilder();

var provider = services.BuildServiceProvider();

var jobBuilder = provider.GetRequiredService<JobBuilder>();

jobBuilder.ConfigureRepository(
    new NpgsqlJobStepRepository("Server=127.0.0.1;Port=5432;Database=test;User Id=test;Password=test;"));
var job = jobBuilder
    .WithName("SampleJob")
    .Next(new SimpleStep("1"))
    .Next(new SimpleStep("2"))
    .Next(new SimpleStep("3"))
    .Next(new SimpleStep("4"))
    .Build();

job.Execute();

internal class SimpleStep : IStep
{
    public SimpleStep()
    {
        
    }

    public SimpleStep(string stepName)
    {
        Name = stepName;
    }
    
    public string Name { get; set; } = "SimpleStep";
    public DateTimeOffset StartTime { get; set; }
    public DateTimeOffset EndTime { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
    public Guid StepId { get; set; } = Guid.CreateVersion7();
    public Guid JobId { get; set; }
    public string? Exception { get; set; }
    public string? Message { get; set; }
    public StepStatus Status { get; set; }
    public StepResult? StepResult { get; set; }
    
    public StepResult Execute()
    {
        var number = Random.Shared.Next(1, 10);

        Task.Delay(number * 500);

        StepResult = number switch
        {
            < 2 => StepResult.Failed("Failed", new ApplicationException("Failed")),
            <= 8 => StepResult.Success(),
            _ => StepResult.Invalid("Invalid")
        };

        return StepResult;
    }
}