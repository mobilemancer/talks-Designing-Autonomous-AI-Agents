namespace AgentsDemo.Scenarios;

public abstract class ScenarioBase : IScenario
{
    public abstract string Name { get; }

    public string endpoint =
        Environment.GetEnvironmentVariable("talks-autonomous-agents-foundry-uri")
        ?? throw new InvalidOperationException("Missing Azure OpenAI endpoint.");

    public string apiKey =
        Environment.GetEnvironmentVariable("talks-autonomous-agents-foundry-key")
        ?? throw new InvalidOperationException("Missing Azure OpenAI key.");

    public async Task RunAsync()
    {
        var stopwatch = Stopwatch.StartNew();
        Console.WriteLine($"=== {Name} ===");
        await ExecuteAsync().ConfigureAwait(false);
        stopwatch.Stop();
        Console.WriteLine($"=== Completed in {stopwatch.Elapsed} ===");
    }

    protected abstract Task ExecuteAsync();
}
