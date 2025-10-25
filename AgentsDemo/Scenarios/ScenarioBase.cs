using System.Diagnostics;

namespace AgentsDemo.Scenarios;

public abstract class ScenarioBase : IScenario
{
    public abstract string Name { get; }

    public void Run()
    {
        var stopwatch = Stopwatch.StartNew();
        Console.WriteLine($"=== {Name} ===");
        Execute();
        stopwatch.Stop();
        Console.WriteLine($"=== Completed in {stopwatch.Elapsed} ===");
    }

    protected abstract void Execute();
}
