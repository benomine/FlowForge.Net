namespace FlowForge.Net;

/// <inheritdoc />
public interface IJobStepRepository
{
    /// <inheritdoc />
    int SaveStep(IStep step);
    /// <inheritdoc />
    int UpdateStep(IStep step);
    /// <inheritdoc />
    List<IStep> GetStepsById<T>(Guid jobId, Guid stepId) where T : IStep, new();
    /// <inheritdoc />
    List<IStep> GetStepsByName<T>(Guid jobId, string stepName) where T : IStep, new();
    /// <inheritdoc />
    void Init();
    /// <inheritdoc />
    void SaveJob(Job job);
    /// <inheritdoc />
    void UpdateJob(Job job);
}