using AgentsDemo.Scenarios;
using AgentsDemo.Services;

var scenarios = new IScenario[]
{
    new SimpleChatCompletion(),
    new PassingImagesToAgents(),
    new MultiTurnChat(),
    new SimpleTool(),
    new AgentAsTool(),
    new MCPTools(),
};

var runner = new ScenarioRunner(scenarios);
await runner.RunAsync();
