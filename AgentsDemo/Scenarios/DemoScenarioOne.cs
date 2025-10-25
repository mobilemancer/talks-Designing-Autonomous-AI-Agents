namespace AgentsDemo.Scenarios;

public sealed class DemoScenarioOne : ScenarioBase
{
    public override string Name => "Scenario One";

    protected override void Execute()
    {
        Console.WriteLine("Running the first demo logic.");
    }
}
