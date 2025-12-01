namespace AgentsDemo.Scenarios;

public sealed class Middleware : ScenarioBase
{
    public override string Name => "Middleware";

    protected override async Task ExecuteDemoAsync()
    {
        AIAgent agent = new AzureOpenAIClient(
            new Uri(endpoint),
            new System.ClientModel.ApiKeyCredential(apiKey)
        )
            .GetChatClient(ModelHelper.GetDeploymentName(Model.GPT41)) //chose your model
            .CreateAIAgent(
                name: "Professional humorist",
                instructions: "You are good at telling jokes.",
                tools: [AIFunctionFactory.Create(GetWeather)]
            );

        agent = agent
            .AsBuilder()
            .Use(runFunc: CustomAgentRunMiddleware, runStreamingFunc: null)
            .Use(CustomFunctionCallingMiddleware)
            .Build();

        var response = await agent.RunAsync("What's the weather in Malmö");

        Console.WriteLine(response);
    }

    async Task<AgentRunResponse> CustomAgentRunMiddleware(
        IEnumerable<ChatMessage> messages,
        AgentThread? thread,
        AgentRunOptions? options,
        AIAgent innerAgent,
        CancellationToken cancellationToken
    )
    {
        Console.WriteLine($"Input: {messages.Count()}");
        var response = await innerAgent
            .RunAsync(messages, thread, options, cancellationToken)
            .ConfigureAwait(false);
        Console.WriteLine($"Output: {response.Messages.Count}");
        return response;
    }

    async ValueTask<object?> CustomFunctionCallingMiddleware(
        AIAgent agent,
        FunctionInvocationContext context,
        Func<FunctionInvocationContext, CancellationToken, ValueTask<object?>> next,
        CancellationToken cancellationToken
    )
    {
        Console.WriteLine($"Function Name: {context!.Function.Name}");
        var result = await next(context, cancellationToken);
        Console.WriteLine($"Function Call Result: {result}");

        return result;
    }

    [Description("Get the weather for a given location.")]
    static string GetWeather([Description("The location to get the weather for.")] string location)
    {
        return $"The weather in {location} is 10 degrees, nice and sunny.";
    }
}
