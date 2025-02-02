namespace FlowForge.Net;

/// <inheritdoc />
public class StepResult
{
    /// <inheritdoc />
    public StepStatus Status { get; private set; }
    /// <inheritdoc />
    public string? Message { get; private set; }
    /// <inheritdoc />
    public Exception? Exception { get; private set; }

    private StepResult(StepStatus status, string? message = null, Exception? exception = null)
    {
        Status = status;
        Message = message;
        Exception = exception;
    }

    /// <inheritdoc />
    public static StepResult Success()
    {
        return new StepResult(StepStatus.Completed);
    }

    /// <inheritdoc />
    public static StepResult Failed(string message, Exception? exception = null)
    {
        return new StepResult(StepStatus.Failed, message, exception);
    }

    /// <inheritdoc />
    public static StepResult Invalid(string message)
    {
        return new StepResult(StepStatus.Invalid, message);
    }
}
