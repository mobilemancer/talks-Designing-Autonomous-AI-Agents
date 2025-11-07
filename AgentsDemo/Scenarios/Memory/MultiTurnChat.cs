namespace AgentsDemo.Scenarios;

public sealed class MultiTurnChat : ScenarioBase
{
    public override string Name => "Multi turn chat";

    protected override async Task ExecuteAsync()
    {
        AIAgent agent = new AzureOpenAIClient(
            new Uri(endpoint),
            new System.ClientModel.ApiKeyCredential(apiKey)
        )
            .GetChatClient(ModelHelper.GetDeploymentName(Model.GPT41))
            .CreateAIAgent(
                name: "Professional humorist",
                instructions: "You are good at telling jokes."
            );

        AgentThread thread = agent.GetNewThread();

        var prompt = "Tell me a joke about programmers.";
        Console.WriteLine(prompt);
        var response = await agent.RunAsync(prompt, thread);
        Console.WriteLine(response);

        prompt = "Now tell me why that's funny.";
        Console.WriteLine(prompt);
        response = await agent.RunAsync(prompt, thread);
        Console.WriteLine(response);

        // Console.WriteLine("---");
        // Console.WriteLine("Thread as JSON:");
        // Console.WriteLine(thread.Serialize(JsonSerializerOptions.Web));
    }
}
