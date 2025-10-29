namespace AgentsDemo.Scenarios;

public sealed class MCPTools : ScenarioBase
{
    public override string Name => "MCP as Tools";

    protected override async Task ExecuteAsync()
    {
        // Create an MCPClient for the GitHub server
        await using var mcpClient = await McpClient.CreateAsync(new StdioClientTransport(new()
        {
            Name = "MCPServer",
            Command = "npx",
            Arguments = ["-y", "--verbose", "@modelcontextprotocol/server-github"],
        }));

        // Retrieve the list of tools available on the GitHub server
        var mcpTools = await mcpClient.ListToolsAsync().ConfigureAwait(false);


        AIAgent agent = new AzureOpenAIClient(
            new Uri(endpoint),
            new System.ClientModel.ApiKeyCredential(apiKey)
        )
            .GetChatClient(ModelCatalog.GetDeploymentName(Model.GPT41))
            .CreateAIAgent(
                name: "A chat client",
                instructions: "You answer questions related to GitHub repositories only.",
                tools: [.. mcpTools.Cast<AITool>()]);

        AgentThread thread = agent.GetNewThread();

        var prompt = "Tell me the about agent in github.";
        Console.WriteLine(prompt);
        var response = await agent.RunAsync(prompt, thread);
        Console.WriteLine(response);
    }

}
