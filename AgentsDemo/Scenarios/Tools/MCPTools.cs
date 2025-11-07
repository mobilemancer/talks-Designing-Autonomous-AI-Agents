namespace AgentsDemo.Scenarios;

public sealed class MCPTools : ScenarioBase
{
    public override string Name => "MCP as Tools";

    protected override async Task ExecuteAsync()
    {
        (McpClient mcpClient, IList<McpClientTool> mcpTools) = await GetMCPTools()
            .ConfigureAwait(false);

        AIAgent agent = new AzureOpenAIClient(
            new Uri(endpoint),
            new System.ClientModel.ApiKeyCredential(apiKey)
        )
            .GetChatClient(ModelHelper.GetDeploymentName(Model.GPT4o))
            .CreateAIAgent(
                name: "A chat client",
                instructions: "You answer questions related to GitHub repositories only.",
                tools: [.. mcpTools.Cast<AITool>()]
            );

        var prompt = "List public repos in github for the user 'mobilemancer'";
        Console.WriteLine("---");
        Console.WriteLine(prompt);
        var response = await agent.RunAsync(prompt, agent.GetNewThread());
        Console.WriteLine(response);
    }

    private static async Task<(McpClient mcpClient, IList<McpClientTool> mcpTools)> GetMCPTools()
    {
        // Create an MCPClient for the GitHub server
        await using var mcpClient = await McpClient.CreateAsync(
            new StdioClientTransport(
                new()
                {
                    Name = "MCPServer",
                    Command = "npx",
                    Arguments = ["-y", "--verbose", "@modelcontextprotocol/server-github"],
                }
            )
        );

        // Retrieve the list of tools available on the GitHub server
        var mcpTools = await mcpClient.ListToolsAsync().ConfigureAwait(false);
        Console.WriteLine("---");
        Console.WriteLine("Tools available from the MCP server");
        foreach (var t in mcpTools)
        {
            Console.WriteLine(t.Name);
        }
        Console.WriteLine("---");

        return (mcpClient, mcpTools);
    }
}
