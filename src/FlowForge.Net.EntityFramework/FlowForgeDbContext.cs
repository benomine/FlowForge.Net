using Microsoft.EntityFrameworkCore;

namespace FlowForge.Net.EntityFramework;

public class FlowForgeDbContext : DbContext, IJobStepRepository
{
    public DbSet<Job> Jobs { get; set; }
    public DbSet<Step> Steps { get; set; }

    public FlowForgeDbContext(DbContextOptions<FlowForgeDbContext> config) : base(config)
    {
        
    }

    public async Task<int> SaveStepAsync(Step step)
    {
        Steps.Add(step);
        return await SaveChangesAsync();
    }

    public async Task<int> UpdateStepAsync(Step step)
    {
        var originalStep = Steps.FirstOrDefault(x => x.StepId == step.StepId);
        if (originalStep == null)
        {
            return 0;
        }
        
        originalStep.Name = step.Name;
        originalStep.Exception = step.Exception;
        originalStep.Message = step.Message;
        originalStep.Status = step.Status;
        originalStep.StartTime = step.StartTime;
        originalStep.EndTime = step.EndTime;
        originalStep.UpdatedAt = step.UpdatedAt;
        
        return await SaveChangesAsync();
    }

    public async Task<List<Step>> GetStepsById<T>(Guid jobId, Guid stepId) where T : Step, new()
    {
        return await Steps.Where(x => x.StepId == stepId && x.JobId == jobId).ToListAsync();
    }

    public async Task<List<Step>> GetStepsByName<T>(Guid jobId, string stepName) where T : Step, new()
    {
        return await Steps.Where(x => x.Name == stepName && x.JobId == jobId).ToListAsync();
    }

    public async Task Init()
    {
        await Task.FromResult("");
    }

    public async Task<int> SaveJobAsync(Job job)
    {
        Jobs.Add(job);
        return await SaveChangesAsync();
    }

    public async Task<int> UpdateJobAsync(Job job)
    {
        var originalJob = Jobs.FirstOrDefault(x => x.JobId == job.JobId);
        if (originalJob == null)
        {
            return 0;
        }
        
        originalJob.Status = job.Status;
        
        return await SaveChangesAsync();
    }
}