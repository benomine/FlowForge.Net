namespace FlowForge.Net;

public class Step
{
    public string Name { get; set; } = default!;
    public DateTimeOffset StartTime { get; set; }
    public DateTimeOffset EndTime { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
    public Guid StepId { get; set; }
    public Guid JobId { get; set; }
    public string? Exception { get; set; }
    public string? Message { get; set; }
    public StepStatus Status { get; set; }
    public StepResult? StepResult { get; set; }

    public Task<StepResult> ExecuteAsync()
    {
        return Task.FromResult(StepResult.Success());
    }
}
