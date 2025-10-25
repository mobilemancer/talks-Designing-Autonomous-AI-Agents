namespace AgentsDemo.Scenarios;

public sealed class DemoScenarioThree : ScenarioBase
{
    public override string Name => "Scenario Three";

    protected override void Execute()
    {
        Console.WriteLine("Running the third demo logic.");
    }
}
