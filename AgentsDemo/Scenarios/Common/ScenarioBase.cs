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

        Console.ForegroundColor = ConsoleColor.DarkCyan;
        Console.WriteLine($"\n~== {Name} ==~\n");
        Console.ForegroundColor = ConsoleColor.White;

        await ExecuteAsync().ConfigureAwait(false);

        stopwatch.Stop();

        Console.ForegroundColor = ConsoleColor.DarkCyan;
        Console.WriteLine($"\n~== Demo Completed in {stopwatch.Elapsed} ==~\n");
        Console.ForegroundColor = ConsoleColor.White;
    }

    protected abstract Task ExecuteAsync();
}
