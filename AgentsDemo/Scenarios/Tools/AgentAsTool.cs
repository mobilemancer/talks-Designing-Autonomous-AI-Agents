namespace AgentsDemo.Scenarios;

public sealed class AgentAsTool : ScenarioBase
{
    public override string Name => "Agent as Tool";

    protected override async Task ExecuteAsync()
    {
        AIAgent weatherAgent = new AzureOpenAIClient(
            new Uri(endpoint),
            new System.ClientModel.ApiKeyCredential(apiKey)
        )
            .GetChatClient(ModelHelper.GetDeploymentName(Model.GPT41mini))
            .CreateAIAgent(
                name: "Weather reporter",
                instructions: "Report the weather for a given location requested by the user. Use tools",
                tools: [AIFunctionFactory.Create(GetWeather)]
            );

        AIAgent agent = new AzureOpenAIClient(
            new Uri(endpoint),
            new System.ClientModel.ApiKeyCredential(apiKey)
        )
            .GetChatClient(ModelHelper.GetDeploymentName(Model.GPT41))
            .CreateAIAgent(
                name: "A chat client",
                instructions: "You are a helpful agent",
                tools: [weatherAgent.AsAIFunction()]
            );

        AgentThread thread = agent.GetNewThread();

        var prompt = "Tell me the weather in Malmö.";
        Console.WriteLine(prompt);
        var response = await agent.RunAsync(prompt, thread);
        Console.WriteLine(response);
    }

    [Description("Get the weather for a given location.")]
    static string GetWeather([Description("The location to get the weather for.")] string location)
    {
        Console.WriteLine($"Weather requested for {location}.");
        return location == "Malmö"
            ? "The weather in Malmö is 5 degrees but still awesome!"
            : $"The weather in {location} cold and booring.";
    }
}
