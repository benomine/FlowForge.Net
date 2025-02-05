using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace FlowForge.Net;

public class JobBuilder
{
    private readonly List<Step> _steps;
    private IJobStepRepository? _repository;
    private readonly ILogger<Job> _logger;
    private readonly Guid _jobId;
    private string? _jobName;
    private Step? _firstStep;

    public JobBuilder(ILogger<Job> logger)
    {
        _logger = logger;
        _steps = [];
        _jobId = Guid.CreateVersion7();
    }

    public void UseNpgsql(string connectionString)
    {
        _repository = new NpgsqlJobStepRepository(connectionString);
    }
    
    public JobBuilder Next(Step step)
    {
        step.JobId = _jobId;
        step.Status = StepStatus.Pending;
        step.StepId = Guid.CreateVersion7();
        _steps.Add(step);
        return this;
    }

    public JobBuilder StartWith(Step step)
    {
        if (_firstStep is not null)
        {
            throw new InvalidOperationException("Cannot call StartWith multiple times.");
        }
        
        _firstStep = step;
        _firstStep.JobId = _jobId;
        _firstStep.Status = StepStatus.Pending;
        _firstStep.StepId = Guid.CreateVersion7();
        return this;
    }

    public JobBuilder WithName(string name)
    {
        _jobName = name;
        return this;
    }

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