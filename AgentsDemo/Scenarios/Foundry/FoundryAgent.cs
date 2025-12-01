using Azure.AI.Projects;
using Azure.AI.Projects.OpenAI;
using OpenAI.Responses;

namespace AgentsDemo.Scenarios;

public sealed class FoundryAgent : ScenarioBase
{
    public override string Name => "Foundry Defined Agent Producing CFPs";


    const string projectEndpoint = "https://autonomous-agents.services.ai.azure.com/api/projects/firstProject";
    const string agentName = "abstract-writer";

    protected override async Task ExecuteDemoAsync()
    {
        // Connect to your project using the endpoint from your project page
        AIProjectClient projectClient = new(endpoint: new Uri(projectEndpoint), tokenProvider: new DefaultAzureCredential(includeInteractiveCredentials: true));

        // Create a conversation first - this is required!
        ProjectConversation conversation = projectClient.OpenAI.Conversations.CreateProjectConversation();

        // Use AgentReference and pass the conversation ID to the response client
        AgentReference agentReference = new AgentReference(name: agentName);
        ProjectResponsesClient responseClient = projectClient.OpenAI.GetProjectResponsesClientForAgent(agentReference, conversation.Id);

        OpenAIResponse response = responseClient.CreateResponse(AskUserForTopic());
        Console.WriteLine(response.GetOutputText());
    }

    private static string? AskUserForTopic()
    {
        Console.WriteLine();
        Console.WriteLine(new string('-', 80));
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine(">> MISSION: Craft a conference-worthy CFP!");
        Console.ResetColor();
        Console.WriteLine(new string('-', 80));
        Console.Write("\n   What topic shall we write about? ");
        var topic = Console.ReadLine();
        Console.WriteLine();
        return topic;
    }
}