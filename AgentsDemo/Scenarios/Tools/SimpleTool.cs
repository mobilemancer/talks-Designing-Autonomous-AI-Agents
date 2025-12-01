namespace AgentsDemo.Scenarios;

public sealed class SimpleTool : ScenarioBase
{
    public override string Name => "Simple tool calling";

    protected override async Task ExecuteDemoAsync()
    {
        AIAgent agent = new AzureOpenAIClient(
            new Uri(endpoint),
            new System.ClientModel.ApiKeyCredential(apiKey)
        )
            .GetChatClient(ModelHelper.GetDeploymentName(Model.GPT41))
            .CreateAIAgent(
                name: "Weather reporter",
                instructions: "Report the weather for a given location requested by the user. Use tools",
                tools: [AIFunctionFactory.Create(GetWeather)]
            );

        AgentThread thread = agent.GetNewThread();

        var prompt = "Tell me the weather in Manchester.";
        Console.WriteLine(prompt);
        var response = await agent.RunAsync(prompt, thread);
        Console.WriteLine(response);
    }

    [Description("Get the weather for a given location.")]
    static string GetWeather([Description("The location to get the weather for.")] string location)
    {
        Console.WriteLine($"Weather requested for {location}.");
        return location == "Manchester"
            ? "The weather in Manchester is 5 degrees but still awesome!"
            : $"The weather in {location} cold and booring.";
    }
}
