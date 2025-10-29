namespace AgentsDemo.Scenarios;

public sealed class SimpleChatCompletion : ScenarioBase
{
    public override string Name => "Simple Chat Agent";

    protected override async Task ExecuteAsync()
    {
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
