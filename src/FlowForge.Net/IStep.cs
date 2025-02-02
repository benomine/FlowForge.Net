namespace FlowForge.Net;

/// <inheritdoc />
public interface IStep
{
    /// <inheritdoc />
    public string Name { get; set; }
    /// <inheritdoc />
    public DateTimeOffset StartTime { get; set; }
    /// <inheritdoc />
    public DateTimeOffset EndTime { get; set; }
    /// <inheritdoc />
    public DateTimeOffset CreatedAt { get; set; }
    /// <inheritdoc />
    public DateTimeOffset UpdatedAt { get; set; }
    /// <inheritdoc />
    public Guid StepId { get; set; }
    /// <inheritdoc />
    public Guid JobId { get; set; }
    /// <inheritdoc />
    public string? Exception { get; set; }
    /// <inheritdoc />
    public string? Message { get; set; }
    /// <inheritdoc />
    public StepStatus Status { get; set; }
    /// <inheritdoc />
    public StepResult? StepResult { get; set; }
    
    /// <inheritdoc />
    public StepResult Execute();
}
