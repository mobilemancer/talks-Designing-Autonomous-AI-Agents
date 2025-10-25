namespace AgentsDemo.Scenarios;

public interface IScenario
{
    string Name { get; }
    void Run();
}
