namespace AgentsDemo.Scenarios;

using System.IO;

public sealed class Metagognition : ScenarioBase
{
    public override string Name => "Metacognition";

    protected override async Task ExecuteAsync()
    {
        AIAgent agent = new AzureOpenAIClient(
            new Uri(endpoint),
            new System.ClientModel.ApiKeyCredential(apiKey)
        )
            .GetChatClient(ModelCatalog.GetDeploymentName(Model.GPT41))
            .CreateAIAgent(
                name: "Fact gatherer",
                instructions: @"
                You are a senior content researcher and editor, helping the user gather information.
                - Before ouputing research content to the user, make sure to do it according to their preferences.
                - Use search tools to retrieve content.
                - Update users preferences when needed.",
                tools:
                [
                    AIFunctionFactory.Create(get_user_preferences),
                    AIFunctionFactory.Create(save_user_preferences),
                    SearchTool().AsAIFunction(),
                ]
            );

        AgentThread thread = agent.GetNewThread();

        while (true)
        {
            Console.Write("Enter your prompt (or 'exit' to quit): ");
            var prompt = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(prompt) || prompt.ToLower() == "exit")
                break;

            Console.WriteLine($"User: {prompt}");
            var response = await agent.RunAsync(prompt, thread);
            Console.WriteLine($"Agent: {response}");
        }
    }

    [Description("Get the users preferences.")]
    static string get_user_preferences()
    {
        Console.WriteLine($"Getting user preferences");

        string path = Path.Combine(".data", "preferences.md");
        if (File.Exists(path))
        {
            return File.ReadAllText(path);
        }
        return "No preferences found.";
    }

    [Description("Save the users preferences.")]
    static string save_user_preferences(string preferences)
    {
        Console.WriteLine($"Saving user preferences");

        Directory.CreateDirectory(".data");
        File.WriteAllText(Path.Combine(".data", "preferences.md"), preferences);
        return "Preferences saved.";
    }

    [Description("Search for things.")]
    AIAgent SearchTool()
    {
        Console.WriteLine($"Calling search agent");

        AIAgent agent = new AzureOpenAIClient(
            new Uri(endpoint),
            new System.ClientModel.ApiKeyCredential(apiKey)
        )
            .GetChatClient(ModelCatalog.GetDeploymentName(Model.GPT41))
            .CreateAIAgent(
                name: "Search agent",
                instructions: "You are simulating search by returning a long rant on the users subject"
            );
        return agent;
    }
}
