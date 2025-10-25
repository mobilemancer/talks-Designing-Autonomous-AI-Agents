using System.Diagnostics;

namespace AgentsDemo.Scenarios;

public abstract class ScenarioBase : IScenario
{
    public abstract string Name { get; }

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
