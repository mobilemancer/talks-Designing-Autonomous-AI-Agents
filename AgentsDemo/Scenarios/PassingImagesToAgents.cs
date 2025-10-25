namespace AgentsDemo.Scenarios;

public sealed class PassingImagesToAgents : ScenarioBase
{
    public override string Name => "Passing images to agents";

    protected override async Task ExecuteAsync()
    {
        Console.WriteLine("Passing images to an agent.");

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
            .GetChatClient(ModelCatalog.GetDeploymentName(Model.GPT4o)) //chose your model
            .CreateAIAgent(
                name: "Vision Agent",
                instructions: "You are a helpful agent that can analyze images"
            );

        ChatMessage message =
            new(
                ChatRole.User,
                [
                    new TextContent(
                        "Analyze this image and tell me what you see, then tell me a joke about this image"
                    ),
                    new UriContent(
                        "https://substackcdn.com/image/fetch/f_auto,q_auto:good,fl_progressive:steep/https://bucketeer-e05bbc84-baa3-437e-9518-adb32be77984.s3.amazonaws.com/public/images/22ac4184-29b9-492e-897d-2ea385851066_1117x1852.jpeg",
                        "image/jpeg"
                    ),
                ]
            );

        var response = await agent.RunAsync(message);

        Console.WriteLine(response);
    }
}
