using System.Threading.Tasks;

namespace AgentsDemo.Scenarios;

public sealed class MultiTurnChat2 : ScenarioBase
{
    public override string Name => "Scenario Three";

    protected override Task ExecuteAsync()
    {
        Console.WriteLine("Running the third demo logic.");
        return Task.CompletedTask;
    }
}
