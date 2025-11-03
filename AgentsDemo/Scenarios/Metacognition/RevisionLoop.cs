// namespace AgentsDemo.Scenarios;

// // ------------ Domain models ------------
// public record Step(string Name, string Instruction);

// public record StepResult(string Step, string Output, double Confidence, string Rationale);

// public record OverallAssessment(double OverallConfidence, bool IsComplete, string Summary);

// // ------------ Demo app ------------

// public sealed class RevisionLoop : ScenarioBase
// {
//     public override string Name => "Metacognition";

//     // “User memory” that we pass as context across runs (you can update this anytime).
//     static readonly Dictionary<string, string> UserMemory =
//         new()
//         {
//             ["userName"] = "Andreas",
//             ["audience"] = "Conference attendees",
//             ["style"] = "Concise but confident",
//         };

//     static readonly Step[] Plan =
//     {
//         new(
//             "Gather info",
//             "List 3 current, high-signal talking points about AI metacognition demos."
//         ),
//         new("Create copy", "Draft a 3-bullet, stage-ready script (<=15 words per bullet)."),
//         new("Design visuals", "Suggest 2 slide titles with 1-liner captions for each."),
//     };

//     // System prompt: tell the agent to act + self-evaluate with JSON
//     static readonly string MetaInstructions = """
// You are a metacognitive assistant that must:
// 1) Execute the requested step for a short conference demo about AI metacognition.
// 2) Self-evaluate that step against these criteria:
//    - Relevance to the user's goal and audience
//    - Clarity and brevity (conference-friendly)
//    - Actionability (immediately usable on stage)
// Return STRICT JSON:
// {
//   "step": "<step name>",
//   "output": "<concise result>",
//   "confidence": <0..1 number>,
//   "rationale": "<why this confidence>"
// }
// Never include extra text outside JSON.
// """;

//     // A follow-up prompt to judge overall “done vs not done”
//     static readonly string OverallJudge = """
// You are a metacognitive judge. You receive an array of step results (JSON).
// Decide if the task is complete now. Consider: Are any steps low confidence (<0.75)? Is there an obvious gap?
// Return STRICT JSON:
// {
//   "overallConfidence": <0..1>,
//   "isComplete": <true|false>,
//   "summary": "<one-sentence justification>"
// }
// Only JSON. No extra text.
// """;

//     protected override async Task ExecuteAsync()
//     {
//         var agent = new AzureOpenAIClient(
//             new Uri(endpoint),
//             new System.ClientModel.ApiKeyCredential(apiKey)
//         )
//             .GetChatClient(ModelHelper.GetDeploymentName(Model.GPT41))
//             .CreateAIAgent(name: "MetaJudge", instructions: MetaInstructions);
//         ;

//         // One thread to hold our conversation state
//         var thread = agent.GetNewThread();

//         var stepResults = new List<StepResult>();

//         foreach (var step in Plan)
//         {
//             // Put user memory + the current step into the message
//             var payload = new
//             {
//                 userMemory = UserMemory,
//                 step = step.Name,
//                 task = step.Instruction,
//             };

//             await agent.RunAsync(
//                 new ChatMessage(
//                     ChatRole.System,
//                     $"```json\n{JsonSerializer.Serialize(payload)}\n```"
//                 ),
//                 thread
//             );

//             // Run with optional extra instruction (reinforce persona/style)
//             var run = await agent.RunAsync(
//                 $"Consider audience: {UserMemory["audience"]}. Style: {UserMemory["style"]}.",
//                 thread
//             );

//             // Poll until done (or error)
//             run = await PollAsync(agent, thread, run);

//             // Fetch the latest agent message (expected strict JSON)
//             var json = await GetLastAssistantJsonAsync(client, thread);
//             var result =
//                 JsonSerializer.Deserialize<StepResult>(
//                     json,
//                     new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
//                 ) ?? throw new InvalidOperationException("Bad JSON from agent.");
//             stepResults.Add(result);

//             Console.WriteLine($"\n=== {result.Step} ===");
//             Console.WriteLine(result.Output);
//             Console.WriteLine($"Confidence: {result.Confidence:0.00}");
//             Console.WriteLine($"Why: {result.Rationale}");
//         }

//         // Ask the agent to compute overall completion judgment
//         var overallInput = JsonSerializer.Serialize(stepResults);
//         await client.Messages.CreateMessageAsync(
//             thread.Id,
//             MessageRole.User,
//             content: $"Judge overall completion for these step results:\n```json\n{overallInput}\n```"
//         );

//         var judgeRun = await client.Runs.CreateRunAsync(
//             thread.Id,
//             agent.Id,
//             additionalInstructions: OverallJudge
//         );
//         judgeRun = await PollAsync(client, thread, judgeRun);
//         var overallJson = await GetLastAssistantJsonAsync(client, thread);

//         var overall =
//             JsonSerializer.Deserialize<OverallAssessment>(
//                 overallJson,
//                 new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
//             ) ?? throw new InvalidOperationException("Bad overall JSON.");

//         Console.WriteLine("\n====== OVERALL ======");
//         Console.WriteLine(
//             $"IsComplete: {overall.IsComplete}  |  OverallConfidence: {overall.OverallConfidence:0.00}"
//         );
//         Console.WriteLine($"Summary: {overall.Summary}");

//         // Example: update memory based on result (metacog control)
//         if (!overall.IsComplete)
//         {
//             UserMemory["style"] = "More detailed with examples"; // adapt going forward
//             Console.WriteLine("\n[Memory] Updated style preference to: " + UserMemory["style"]);
//         }
//     }

//     // ------ Helpers ------
//     static async Task<ThreadRun> PollAsync(
//         PersistentAgentsClient client,
//         PersistentAgentThread thread,
//         ThreadRun run
//     )
//     {
//         while (run.Status is RunStatus.Queued or RunStatus.InProgress or RunStatus.RequiresAction)
//         {
//             await Task.Delay(400);
//             run = await client.Runs.GetRunAsync(thread.Id, run.Id);
//             if (run.Status is RunStatus.Failed or RunStatus.Expired or RunStatus.Canceled)
//             {
//                 throw new RequestFailedException(run.LastError?.Message ?? "Run failed");
//             }
//         }
//         return run;
//     }

//     static async Task<string> GetLastAssistantJsonAsync(
//         PersistentAgentsClient client,
//         PersistentAgentThread thread
//     )
//     {
//         var messages = client.Messages.GetMessagesAsync(
//             threadId: thread.Id,
//             order: ListSortOrder.Descending
//         );
//         await foreach (var m in messages)
//         {
//             if (m.Role == MessageRole.Agent)
//             {
//                 foreach (var content in m.ContentItems)
//                 {
//                     if (content is MessageTextContent t)
//                         return t.Text.Trim();
//                 }
//             }
//         }
//         throw new InvalidOperationException("No agent message found.");
//     }
// }
