using FlowForge.Net;

internal class SimpleStep : Step
{
    public SimpleStep()
    {
        Name = nameof(SimpleStep);
    }

    public SimpleStep(string stepName)
    {
        Name = stepName;
    }
    
    public new async Task<StepResult> ExecuteAsync()
    {
        var number = Random.Shared.Next(1, 10);

        await Task.Delay(number * 500);

        StepResult = number switch
        {
            < 2 => StepResult.Failed("Failed", new ApplicationException("Failed")),
            <= 8 => StepResult.Success(),
            _ => StepResult.Invalid("Invalid")
        };

        return StepResult;
    }
}