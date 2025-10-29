using AgentsDemo.Scenarios;
using AgentsDemo.Services;

var scenarios = new IScenario[]
{
    new SimpleChatCompletion(),
    new PassingImagesToAgents(),
    new MultiTurnChat(),
    new SimpleTool(),
};

var runner = new ScenarioRunner(scenarios);
await runner.RunAsync();
