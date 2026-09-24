using Azure;
using Azure.AI.OpenAI;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.AzureOpenAI;
using GitHubTriageAgent.Tools;

Console.ForegroundColor = ConsoleColor.Cyan;
Console.WriteLine("==================================================================");
Console.WriteLine(" 🚀 Microsoft NextGen Heroes - AI Agent Tool Calling Live Demo");
Console.WriteLine("==================================================================\n");
Console.ResetColor();

// 1. Configuration & Credentials
string azureEndpoint = "https://<TU-RECURSO-AZURE>.openai.azure.com/";
string apiKey = "<TU-KEY>";
string deploymentName = "gpt-4o";
string githubToken = "ghp_...";
string owner = "tu-usuario";
string repo = "nextgen-agent-demo-repo";

// 2. Initialize GitHub Tools
var gitHubTools = new GitHubTools(githubToken, owner, repo);

// 3. Build Azure OpenAI Client explicitly with API Version
var azureClientOptions = new AzureOpenAIClientOptions(AzureOpenAIClientOptions.ServiceVersion.V2024_06_01);
var azureClient = new AzureOpenAIClient(new Uri(azureEndpoint), new AzureKeyCredential(apiKey), azureClientOptions);

var builder = Kernel.CreateBuilder();
// Pasamos el cliente ya instanciado al Kernel
builder.AddAzureOpenAIChatCompletion(
    deploymentName: deploymentName,
    azureOpenAIClient: azureClient
);
builder.Plugins.AddFromObject(gitHubTools, pluginName: "GitHubPlugin");

Kernel kernel = builder.Build();
var chatService = kernel.GetRequiredService<IChatCompletionService>();

// 4. System Prompt Defining Agent Persona & Workflow Strategy
string systemPrompt = """
You are 'HeroTriageAgent', an autonomous AI Software Engineer.
Your goal is to investigate GitHub issues, search the codebase for the bug, auto-generate a fix, and open a Pull Request.

ALWAYS follow this step-by-step reasoning loop:
1. Fetch the details of the reported Issue using `GetIssueDetailsAsync`.
2. Search the repository for relevant code files using `SearchRepositoryCodeAsync`.
3. Analyze the bug cause and construct the corrected code.
4. Execute `CreatePullRequestAsync` to submit the fix in a new branch.
5. Post a polite comment on the Issue with `PostIssueCommentAsync` referencing the new PR.

Be concise, precise, and professional.
""";

var chatHistory = new ChatHistory();
chatHistory.AddSystemMessage(systemPrompt);

// 5. User Execution Trigger (Simulating Live Issue Triage)
Console.Write("Enter GitHub Issue Number to Triage & Fix [e.g., 1]: ");
string inputIssue = Console.ReadLine() ?? "1";

string userPrompt = $"Please triage Issue #{inputIssue}. The bug is in Calculator.cs (or search the repository files), fix the Divide method, create a PR with the fix, and update the issue.";
chatHistory.AddUserMessage(userPrompt);

Console.ForegroundColor = ConsoleColor.Yellow;
Console.WriteLine($"\n[AGENT WORKING]: Starting Autonomous Loop for Issue #{inputIssue}...\n");
Console.ResetColor();

/// Enable Automatic Tool Calling (Sintaxis para Semantic Kernel v1.35+)
PromptExecutionSettings settings = new AzureOpenAIPromptExecutionSettings
{
    FunctionChoiceBehavior = FunctionChoiceBehavior.Auto()
};

// 6. Execute & Stream Agent Thought Loop
var response = await chatService.GetChatMessageContentAsync(chatHistory, settings, kernel);

Console.ForegroundColor = ConsoleColor.Green;
Console.WriteLine($"\n[AGENT FINAL REPORT]:\n{response.Content}");
Console.ResetColor();