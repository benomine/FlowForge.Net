using Microsoft.Extensions.Logging;

namespace FlowForge.Net;

/// <inheritdoc />
public class Job
{
    private readonly List<IStep> _steps = [];
    private readonly ILogger<Job> _logger;
    
    private IJobStepRepository _repository = null!;
    /// <inheritdoc />
    public string JobName = string.Empty;
    /// <inheritdoc />
    public Guid JobId { get; set; }
    /// <inheritdoc />
    public JobStatus Status { get; private set; } = JobStatus.Started;

    /// <inheritdoc />
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

    /// <inheritdoc />
    public void Next(IStep step)
    {
        _steps.Add(step);
    }

    /// <inheritdoc />
    public void Execute()
    {
        if (Status is JobStatus.Started)
            _repository.SaveJob(this);

        foreach (var step in _steps)
        {
            _repository.SaveStep(step);

            _logger.LogInformation("Executing step {Name}", step.Name);
            step.CreatedAt = DateTimeOffset.UtcNow;
            step.Status = StepStatus.InProgress;
            
            _repository.UpdateStep(step);
            ExecuteStep(step);
        }

        if (_steps.Any(x => x.StepResult?.Status is StepStatus.Failed or StepStatus.Invalid))
        {
            Status = JobStatus.Failed;
        }
        else
        {
            Status = JobStatus.Finished;
        }
        
        _repository.UpdateJob(this);
    }
    
    private void ExecuteStep(IStep step)
    {
        try
        {
            var result = step.Execute();
            _logger.LogInformation("Executed step {Name}", step.Name);
                
            switch (result.Status)
            {
                case StepStatus.Completed:
                    _logger.LogInformation("Step {Name} succeeded.", step.Name);
                    step.EndTime = DateTimeOffset.UtcNow;
                    step.Status = StepStatus.Completed;
                    break;
                case StepStatus.Failed:
                {
                    if (result.Exception is not null)
                    {
                        _logger.LogError(result.Exception, "Step {Name} failed.", step.Name);
                        step.Message = result.Message;
                        step.Exception = result.Exception.Message;
                    }
                    else
                    {
                        _logger.LogError("Step {Name} failed.", step.Name);
                    }
                        
                    step.Status = StepStatus.Failed;
                    break;
                }
                case StepStatus.Invalid:
                    _logger.LogError("Step {Name} in invalid state.", step.Name);
                    step.Status = StepStatus.Invalid;
                    break;
            }
                
            _repository.UpdateStep(step);
        }
        catch (Exception e)
        {
            var stepResult = StepResult.Failed(e.Message, e);
            step.Status = StepStatus.Failed;
            step.Message = stepResult.Message;
            step.Exception = stepResult.Exception!.Message;
            _repository.UpdateStep(step);
        }
    }

    /// <inheritdoc />
    public void ReplayStep<T>(string stepName) where T : IStep, new()
    {
        var steps = _repository.GetStepsByName<T>(JobId, stepName);
        if (steps.Count == 0)
        {
            _logger.LogError("Unknown step {StepName} for job {JobId}.", stepName, JobId);
            return;
        }

        var stepInstance = steps.First();
        stepInstance.StartTime = DateTimeOffset.UtcNow;
        stepInstance.Status = StepStatus.InProgress;
        ExecuteStep(stepInstance);
    }

    /// <inheritdoc />
    public void ReplayStep<T>(Guid stepId) where T : IStep, new()
    {
        var steps = _repository.GetStepsById<T>(JobId, stepId);
        if (steps.Count == 0)
        {
            _logger.LogError("Unknown step {StepId} for job {JobId}.", stepId, JobId);
            return;
        }
       
        var stepInstance = steps.First();
        stepInstance.StartTime = DateTimeOffset.UtcNow;
        stepInstance.Status = StepStatus.InProgress;
        ExecuteStep(stepInstance);
    }
}