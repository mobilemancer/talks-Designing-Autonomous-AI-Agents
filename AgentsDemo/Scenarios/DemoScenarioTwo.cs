namespace AgentsDemo.Scenarios;

public sealed class DemoScenarioTwo : ScenarioBase
{
    public override string Name => "Scenario Two";

    protected override void Execute()
    {
        Console.WriteLine("Running the second demo logic.");
    }
}
