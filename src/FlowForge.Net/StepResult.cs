namespace FlowForge.Net;

public class StepResult
{
    public StepStatus Status { get; private set; }
    public string? Message { get; private set; }
    public Exception? Exception { get; private set; }

    private StepResult(StepStatus status, string? message = null, Exception? exception = null)
    {
        Status = status;
        Message = message;
        Exception = exception;
    }

    public static StepResult Success()
    {
        return new StepResult(StepStatus.Completed);
    }

    public static StepResult Failed(string message, Exception? exception = null)
    {
        return new StepResult(StepStatus.Failed, message, exception);
    }

    public static StepResult Invalid(string message)
    {
        return new StepResult(StepStatus.Invalid, message);
    }
}
