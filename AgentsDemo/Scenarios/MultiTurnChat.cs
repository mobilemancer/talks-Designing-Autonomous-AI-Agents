namespace AgentsDemo.Scenarios;

using System.Text.Json;
using AgentsDemo.Models;
using Azure.AI.OpenAI;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using OpenAI;

public sealed class MultiTurnChat : ScenarioBase
{
    public override string Name => "Multi turn chat";

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
            .GetChatClient(ModelCatalog.GetDeploymentName(Model.GPT41))
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

        Console.WriteLine("---");

        // Console.WriteLine("Thread as JSON:");
        // Console.WriteLine(thread.Serialize(JsonSerializerOptions.Web));
    }
}
