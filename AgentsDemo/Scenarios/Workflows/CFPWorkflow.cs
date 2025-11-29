namespace AgentsDemo.Scenarios;

public sealed class CFPWorkflow : ScenarioBase
{
    public override string Name => "Workflow producing CFPs";
    public const int MaxIterations = 3;

    protected override async Task ExecuteAsync()
    {

        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine($"~~ The Writer & Critic duo are ready to collaborate! (Up to {MaxIterations} rounds of friendly debate) ~~");
        Console.ResetColor();

        // Set up the Azure OpenAI client
        IChatClient chatClient = new AzureOpenAIClient(
            new Uri(endpoint),
            new System.ClientModel.ApiKeyCredential(apiKey)
        )
        .GetChatClient(ModelHelper.GetDeploymentName(Model.GPT41)).AsIChatClient();

        IChatClient chatClient2 = new AzureOpenAIClient(
            new Uri(endpoint),
            new System.ClientModel.ApiKeyCredential(apiKey)
        )
        .GetChatClient(ModelHelper.GetDeploymentName(Model.GPT51)).AsIChatClient();


        // Create executors for content creation and review
        WriterExecutor writer = new(chatClient);
        CriticExecutor critic = new(chatClient2);
        SummaryExecutor summary = new(chatClient);

        // Build the workflow with conditional routing based on critic's decision
        WorkflowBuilder workflowBuilder = new WorkflowBuilder(writer)
            .AddEdge(writer, critic)
            .AddSwitch(critic, sw => sw
                .AddCase<CriticDecision>(cd => cd?.Approved == true, summary)
                .AddCase<CriticDecision>(cd => cd?.Approved == false, writer))
            .WithOutputFrom(summary);

        // Execute the workflow with a sample task
        // The workflow loops back to Writer if content is rejected,
        // or proceeds to Summary if approved. State tracking ensures we don't loop forever.
        string? topic = AskUserForTopic();

        string InitialTask =
        @$"Write the perfect CFP submission for a tech conference about {topic}. 
         Only the abstract is needed!";

        Workflow workflow = workflowBuilder.Build();
        await ExecuteWorkflowAsync(workflow, InitialTask);

        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("\n[Done] Mission accomplished! The Writer-Critic workflow has spoken.\n");
        Console.ResetColor();
        Console.WriteLine("What you just witnessed:");
        Console.WriteLine("   * Iterative refinement with conditional routing");
        Console.WriteLine("   * Shared workflow state for iteration tracking");
        Console.WriteLine($"   * Safety cap of {MaxIterations} iterations (no infinite loops here!)");
        Console.WriteLine("   * Real-time streaming with structured output\n");
    }

    private static string? AskUserForTopic()
    {
        Console.WriteLine();
        Console.WriteLine(new string('-', 80));
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine(">> MISSION: Craft a conference-worthy CFP that sounds authentically human!");
        Console.ResetColor();
        Console.WriteLine(new string('-', 80));
        Console.Write("\n   What topic shall we write about? ");
        var topic = Console.ReadLine();
        Console.WriteLine();
        return topic;
    }

    private static async Task ExecuteWorkflowAsync(Workflow workflow, string input)
    {
        // Execute in streaming mode to see real-time progress
        await using StreamingRun run = await InProcessExecution.StreamAsync<string>(workflow, input);

        bool hasShownFinalOutput = false;

        // Watch the workflow events
        await foreach (WorkflowEvent evt in run.WatchStreamAsync())
        {
            switch (evt)
            {
                case AgentRunUpdateEvent agentUpdate:
                    // Stream agent output in real-time
                    if (!string.IsNullOrEmpty(agentUpdate.Update.Text))
                    {
                        Console.Write(agentUpdate.Update.Text);
                    }
                    break;

                case WorkflowOutputEvent output:
                    if (!hasShownFinalOutput)
                    {
                        hasShownFinalOutput = true;
                        Console.WriteLine("\n\n" + new string('=', 80));
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine("<< THE CRITIC HAS APPROVED! Here's your polished CFP >>");
                        Console.ResetColor();
                        Console.WriteLine(new string('=', 80));
                        Console.WriteLine();
                        Console.ForegroundColor = ConsoleColor.Cyan;
                        Console.WriteLine(output.Data);
                        Console.ResetColor();
                        Console.WriteLine();
                        Console.WriteLine(new string('=', 80));
                    }
                    break;
            }
        }
    }

}

// ====================================
// Shared State for Iteration Tracking
// ====================================

/// <summary>
/// Tracks the current iteration and conversation history across workflow executions.
/// </summary>
internal sealed class FlowState
{
    public int Iteration { get; set; } = 1;
    public List<ChatMessage> History { get; } = [];
}

/// <summary>
/// Constants for accessing the shared flow state in workflow context.
/// </summary>
internal static class FlowStateShared
{
    public const string Scope = "FlowStateScope";
    public const string Key = "singleton";
}

/// <summary>
/// Helper methods for reading and writing shared flow state.
/// </summary>
internal static class FlowStateHelpers
{
    public static async Task<FlowState> ReadFlowStateAsync(IWorkflowContext context)
    {
        FlowState? state = await context.ReadStateAsync<FlowState>(FlowStateShared.Key, scopeName: FlowStateShared.Scope);
        return state ?? new FlowState();
    }

    public static ValueTask SaveFlowStateAsync(IWorkflowContext context, FlowState state)
        => context.QueueStateUpdateAsync(FlowStateShared.Key, state, scopeName: FlowStateShared.Scope);
}

// ====================================
// Data Transfer Objects
// ====================================

/// <summary>
/// Structured output schema for the Critic's decision.
/// Uses JsonPropertyName and Description attributes for OpenAI's JSON schema.
/// </summary>
[Description("Critic's review decision including approval status and feedback")]
[SuppressMessage("Performance", "CA1812:Avoid uninstantiated internal classes", Justification = "Instantiated via JSON deserialization")]
internal sealed class CriticDecision
{
    [JsonPropertyName("approved")]
    [Description("Whether the content is approved (true) or needs revision (false)")]
    public bool Approved { get; set; }

    [JsonPropertyName("feedback")]
    [Description("Specific feedback for improvements if not approved, empty if approved")]
    public string Feedback { get; set; } = "";

    // Non-JSON properties for workflow use
    [JsonIgnore]
    public string Content { get; set; } = "";

    [JsonIgnore]
    public int Iteration { get; set; }
}

// ====================================
// Custom Executors
// ====================================

/// <summary>
/// Executor that creates or revises content based on user requests or critic feedback.
/// This executor demonstrates multiple message handlers for different input types.
/// </summary>
internal sealed class WriterExecutor : Executor
{
    private readonly AIAgent _agent;

    public WriterExecutor(IChatClient chatClient) : base("Writer")
    {
        _agent = new ChatClientAgent(
            chatClient,
            name: "Writer",
            instructions: """
                You are a skilled writer. Create clear, engaging content.
                If you receive feedback, carefully revise the content to address all concerns.
                Maintain the same topic and length requirements.
                """
        );
    }

    protected override RouteBuilder ConfigureRoutes(RouteBuilder routeBuilder) =>
        routeBuilder
            .AddHandler<string, ChatMessage>(HandleInitialRequestAsync)
            .AddHandler<CriticDecision, ChatMessage>(HandleRevisionRequestAsync);

    /// <summary>
    /// Handles the initial writing request from the user.
    /// </summary>
    private async ValueTask<ChatMessage> HandleInitialRequestAsync(
        string message,
        IWorkflowContext context,
        CancellationToken cancellationToken = default)
    {
        return await HandleAsyncCoreAsync(new ChatMessage(ChatRole.User, message), context, cancellationToken);
    }

    /// <summary>
    /// Handles revision requests from the critic with feedback.
    /// </summary>
    private async ValueTask<ChatMessage> HandleRevisionRequestAsync(
        CriticDecision decision,
        IWorkflowContext context,
        CancellationToken cancellationToken = default)
    {
        string prompt = "Revise the following content based on this feedback:\n\n" +
                       $"Feedback: {decision.Feedback}\n\n" +
                       $"Original Content:\n{decision.Content}";

        return await HandleAsyncCoreAsync(new ChatMessage(ChatRole.User, prompt), context, cancellationToken);
    }

    /// <summary>
    /// Core implementation for generating content (initial or revised).
    /// </summary>
    private async Task<ChatMessage> HandleAsyncCoreAsync(
        ChatMessage message,
        IWorkflowContext context,
        CancellationToken cancellationToken)
    {
        FlowState state = await FlowStateHelpers.ReadFlowStateAsync(context);

        Console.WriteLine($"\n=== Writer (Iteration {state.Iteration}) ===\n");

        StringBuilder sb = new();
        await foreach (AgentRunResponseUpdate update in _agent.RunStreamingAsync(message, cancellationToken: cancellationToken))
        {
            if (!string.IsNullOrEmpty(update.Text))
            {
                sb.Append(update.Text);
                Console.Write(update.Text);
            }
        }
        Console.WriteLine("\n");

        string text = sb.ToString();
        state.History.Add(new ChatMessage(ChatRole.Assistant, text));
        await FlowStateHelpers.SaveFlowStateAsync(context, state);

        return new ChatMessage(ChatRole.User, text);
    }
}

/// <summary>
/// Executor that reviews content and decides whether to approve or request revisions.
/// Uses structured output with streaming for reliable decision-making.
/// </summary>
internal sealed class CriticExecutor : Executor<ChatMessage, CriticDecision>
{
    private readonly AIAgent _agent;

    public CriticExecutor(IChatClient chatClient) : base("Critic")
    {
        _agent = new ChatClientAgent(chatClient, new ChatClientAgentOptions
        {
            Name = "Critic",
            Instructions = """
                You are a constructive critic. Review the content and provide specific feedback.
                Always try to provide actionable suggestions for improvement and strive to identify improvement points.
                Only approve if the content is high quality, clear, and meets the original requirements and you see no improvement points.
                make sure it's not visible to a human that it is written by AI. 
                No emojis allowed, no em-dash allowed.
                
                Provide your decision as structured output with:
                - approved: true if content is good, false if revisions needed
                - feedback: specific improvements needed (empty if approved)
                
                Be concise but specific in your feedback.
                """,
            ChatOptions = new()
            {
                ResponseFormat = ChatResponseFormat.ForJsonSchema<CriticDecision>()
            }
        });
    }

    public override async ValueTask<CriticDecision> HandleAsync(
        ChatMessage message,
        IWorkflowContext context,
        CancellationToken cancellationToken = default)
    {
        FlowState state = await FlowStateHelpers.ReadFlowStateAsync(context);

        Console.ForegroundColor = ConsoleColor.Blue;
        Console.WriteLine($"=== Critic (Iteration {state.Iteration}) ===\n");

        // Use RunStreamingAsync to get streaming updates, then deserialize at the end
        IAsyncEnumerable<AgentRunResponseUpdate> updates = _agent.RunStreamingAsync(message, cancellationToken: cancellationToken);

        // Stream the output in real-time (for any rationale/explanation)
        await foreach (AgentRunResponseUpdate update in updates)
        {
            if (!string.IsNullOrEmpty(update.Text))
            {
                Console.Write(update.Text);
            }
        }
        Console.WriteLine("\n");
        Console.ResetColor();

        // Convert the stream to a response and deserialize the structured output
        AgentRunResponse response = await updates.ToAgentRunResponseAsync(cancellationToken);
        CriticDecision decision = response.Deserialize<CriticDecision>(JsonSerializerOptions.Web);

        Console.WriteLine($"Decision: {(decision.Approved ? "[OK] APPROVED" : "[X] NEEDS REVISION")}");
        if (!string.IsNullOrEmpty(decision.Feedback))
        {
            Console.WriteLine($"Feedback: {decision.Feedback}");
        }
        Console.WriteLine();

        // Safety: approve if max iterations reached
        if (!decision.Approved && state.Iteration >= CFPWorkflow.MaxIterations)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"! Max iterations ({CFPWorkflow.MaxIterations}) reached - auto-approving");
            Console.ResetColor();
            decision.Approved = true;
            decision.Feedback = "";
        }

        // Increment iteration ONLY if rejecting (will loop back to Writer)
        if (!decision.Approved)
        {
            state.Iteration++;
        }

        // Store the decision in history
        state.History.Add(new ChatMessage(ChatRole.Assistant,
            $"[Decision: {(decision.Approved ? "Approved" : "Needs Revision")}] {decision.Feedback}"));
        await FlowStateHelpers.SaveFlowStateAsync(context, state);

        // Populate workflow-specific fields
        decision.Content = message.Text ?? "";
        decision.Iteration = state.Iteration;

        return decision;
    }
}

/// <summary>
/// Executor that yields the final approved content as workflow output.
/// </summary>
internal sealed class SummaryExecutor : Executor<CriticDecision, ChatMessage>
{
    public SummaryExecutor(IChatClient _) : base("Summary") { }

    public override async ValueTask<ChatMessage> HandleAsync(
        CriticDecision message,
        IWorkflowContext context,
        CancellationToken cancellationToken = default)
    {
        // Simply pass through the approved content - no need to call the agent again
        // The content was already polished by Writer and approved by Critic
        ChatMessage result = new(ChatRole.Assistant, message.Content);
        await context.YieldOutputAsync(result, cancellationToken);
        return result;
    }
}