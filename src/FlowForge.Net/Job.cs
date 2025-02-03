using Microsoft.Extensions.Logging;

namespace FlowForge.Net;

public class Job
{
    private readonly List<Step> _steps = [];
    private readonly ILogger<Job> _logger;

    private IJobStepRepository _repository = null!;
    public string JobName = string.Empty;
    public Guid JobId { get; set; }
    public JobStatus Status { get; set; } = JobStatus.Started;

    internal Job(ILogger<Job> logger)
    {
        _logger = logger;
    }

    internal void SetRepository(IJobStepRepository? repository)
    {
        ArgumentNullException.ThrowIfNull(repository);
        _repository = repository;
    }

    internal void SetJobName(string jobName)
    {
        JobName = jobName;
    }

    public void Next(Step step)
    {
        _steps.Add(step);
    }

    public async Task ExecuteAsync()
    {
        if (Status is JobStatus.Started)
        {
            await _repository.SaveJobAsync(this);
        }

        foreach (var step in _steps)
        {
            await _repository.SaveStepAsync(step);

            _logger.LogInformation("Executing step {Name} {StepId}", step.Name, step.StepId);
            step.CreatedAt = DateTimeOffset.UtcNow;
            step.Status = StepStatus.InProgress;

            await _repository.UpdateStepAsync(step);
            await ExecuteStep(step);
        }

        if (_steps.Any(x => x.StepResult?.Status is StepStatus.Failed or StepStatus.Invalid))
        {
            Status = JobStatus.Failed;
        }
        else
        {
            Status = JobStatus.Finished;
        }

        await _repository.UpdateJobAsync(this);
    }

    private async Task ExecuteStep(Step step)
    {
        try
        {
            var result = await step.ExecuteAsync();
            _logger.LogInformation("Executed step {Name} {StepId}", step.Name, step.StepId);

            switch (result.Status)
            {
                case StepStatus.Completed:
                    _logger.LogInformation("Step {Name} {StepId} succeeded.", step.Name, step.StepId);
                    step.EndTime = DateTimeOffset.UtcNow;
                    step.Status = StepStatus.Completed;
                    break;
                case StepStatus.Failed:
                    {
                        if (result.Exception is not null)
                        {
                            _logger.LogError(result.Exception, "Step {Name} {StepId} failed.", step.Name, step.StepId);
                            step.Message = result.Message;
                            step.Exception = result.Exception.Message;
                        }
                        else
                        {
                            _logger.LogError("Step {Name} {StepId} failed.", step.Name, step.StepId);
                        }

                        step.Status = StepStatus.Failed;
                        break;
                    }
                case StepStatus.Invalid:
                    _logger.LogError("Step {Name} {StepId} in invalid state.", step.Name, step.StepId);
                    step.Status = StepStatus.Invalid;
                    break;
            }

            await _repository.UpdateStepAsync(step);
        }
        catch (Exception e)
        {
            var stepResult = StepResult.Failed(e.Message, e);
            step.Status = StepStatus.Failed;
            step.Message = stepResult.Message;
            step.Exception = stepResult.Exception!.Message;
            await _repository.UpdateStepAsync(step);
        }
    }

    public async Task ReplayStep<T>(string stepName) where T : Step, new()
    {
        var steps = await _repository.GetStepsByName<T>(JobId, stepName);
        if (steps.Count == 0)
        {
            _logger.LogError("Unknown step {StepName} for job {JobId}.", stepName, JobId);
            return;
        }

        var stepInstance = steps.First();
        stepInstance.StartTime = DateTimeOffset.UtcNow;
        stepInstance.Status = StepStatus.InProgress;
        await ExecuteStep(stepInstance);
    }

    public async Task ReplayStep<T>(Guid stepId) where T : Step, new()
    {
        var steps = await _repository.GetStepsById<T>(JobId, stepId);
        if (steps.Count == 0)
        {
            _logger.LogError("Unknown step {StepId} for job {JobId}.", stepId, JobId);
            return;
        }

        var stepInstance = steps.First();
        stepInstance.StartTime = DateTimeOffset.UtcNow;
        stepInstance.Status = StepStatus.InProgress;
        await ExecuteStep(stepInstance);
    }
}