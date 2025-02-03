namespace FlowForge.Net;

public interface IJobStepRepository
{
    Task<int> SaveStepAsync(Step step);
    Task<int> UpdateStepAsync(Step step);
    Task<List<Step>> GetStepsById<T>(Guid jobId, Guid stepId) where T : Step, new();
    Task<List<Step>> GetStepsByName<T>(Guid jobId, string stepName) where T : Step, new();
    Task Init();
    Task<int> SaveJobAsync(Job job);
    Task<int> UpdateJobAsync(Job job);
}
