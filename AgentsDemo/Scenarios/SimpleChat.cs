namespace AgentsDemo.Scenarios;

public sealed class SimpleChatCompletion : ScenarioBase
{
    public override string Name => "Scenario One - Simple Chat";

    protected override async Task ExecuteAsync()
    {
        Console.WriteLine("Simple chat agent.");

        var endpoint =
            Environment.GetEnvironmentVariable("talks-autonomous-agents-foundry-uri")
            ?? throw new InvalidOperationException("Missing Azure OpenAI endpoint.");
        var apiKey =
            Environment.GetEnvironmentVariable("talks-autonomous-agents-foundry-key")
            ?? throw new InvalidOperationException("Missing Azure OpenAI key.");

        AIAgent agent = new AzureOpenAIClient(
            new Uri(endpoint),
            new System.ClientModel.ApiKeyCredential(apiKey)
        )
            .GetChatClient(ModelCatalog.GetDeploymentName(Model.GPT41)) //chose your model
            .CreateAIAgent(
                name: "Professional humorist",
                instructions: "You are good at telling jokes."
            );

        var response = await agent.RunAsync("Tell me a joke about programmers.");

        Console.WriteLine(response);
    }
}
