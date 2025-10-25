using AgentsDemo.Scenarios;
using AgentsDemo.Services;

var scenarios = new IScenario[]
{
    new DemoScenarioOne(),
    new DemoScenarioTwo(),
    new DemoScenarioThree(),
};

var runner = new ScenarioRunner(scenarios);
runner.Run();
