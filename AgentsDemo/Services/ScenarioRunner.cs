using AgentsDemo.Scenarios;

namespace AgentsDemo.Services;

public sealed class ScenarioRunner
{
    private readonly IReadOnlyList<IScenario> _scenarios;

    public ScenarioRunner(IEnumerable<IScenario> scenarios)
    {
        _scenarios = scenarios.ToList();
    }

    public async Task RunAsync()
    {
        while (true)
        {
            Console.Clear();
            Console.WriteLine("Choose a demo (0 to exit):");

            for (var i = 0; i < _scenarios.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {_scenarios[i].Name}");
            }

            Console.Write("\n> ");

            if (!int.TryParse(Console.ReadLine(), out var choice))
            {
                continue;
            }

            if (choice == 0)
            {
                return;
            }

            if (choice < 1 || choice > _scenarios.Count)
            {
                continue;
            }

            Console.Clear();
            var scenario = _scenarios[choice - 1];
            await scenario.RunAsync().ConfigureAwait(false);

            Console.WriteLine("\nPress any key to return to the menu...");
            Console.ReadKey(true);
        }
    }
}
