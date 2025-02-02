using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace FlowForge.Net;

/// <inheritdoc />
public class JobBuilder
{
    private readonly List<IStep> _steps;
    private IJobStepRepository? _repository;
    private readonly ILogger<Job> _logger;
    private IStep? _firstStep;
    private Guid _jobId;
    private string? _jobName;
    
    /// <inheritdoc />
    public JobBuilder(ILogger<Job> logger)
    {
        _logger = logger;
        _steps = [];
        _jobId = Guid.CreateVersion7();
    }

    /// <inheritdoc />
    public void ConfigureRepository(IJobStepRepository repository)
    {
        _repository = repository;
    }
    
    /// <inheritdoc />
    public JobBuilder Next(IStep step)
    {
        step.JobId = _jobId;
        step.Status = StepStatus.Pending;
        _steps.Add(step);
        return this;
    }

    /// <inheritdoc />
    public JobBuilder StartWith(IStep step)
    {
        if (_firstStep is not null)
        {
            throw new InvalidOperationException("Cannot call StartWith multiple times.");
        }
        
        _firstStep = step;
        _firstStep.JobId = _jobId;
        _firstStep.Status = StepStatus.Pending;
        return this;
    }

    /// <inheritdoc />
    public JobBuilder WithName(string name)
    {
        _jobName = name;
        return this;
    }

    /// <inheritdoc />
    public Job Build()
    {
        ArgumentNullException.ThrowIfNull(_repository);
        ArgumentNullException.ThrowIfNull(_jobName);
        
        _repository.Init();
        
        var job = new Job(_logger);
        job.JobId = _jobId;
        job.SetRepository(_repository);
        job.SetJobName(_jobName);
        
        if (_firstStep != null)
        {
            job.Next(_firstStep);
        }
        
        foreach (var step in _steps)
        {
            job.Next(step);
        }

        return job;
    }
}