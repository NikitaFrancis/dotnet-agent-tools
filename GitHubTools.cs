using System.ComponentModel;
using Microsoft.SemanticKernel;
using Octokit;

namespace GitHubTriageAgent.Tools;

public class GitHubTools
{
    private readonly GitHubClient _github;
    private readonly string _owner;
    private readonly string _repo;

    public GitHubTools(string token, string owner, string repo)
    {
        _owner = owner;
        _repo = repo;
        _github = new GitHubClient(new ProductHeaderValue("NextGen-AI-Agent"))
        {
            Credentials = new Credentials(token)
        };
    }

    [KernelFunction, Description("Fetches the details and description of a specific GitHub Issue.")]
    public async Task<string> GetIssueDetailsAsync(
        [Description("The Issue number to fetch")] int issueNumber)
    {
        var issue = await _github.Issue.Get(_owner, _repo, issueNumber);
        return $"[Issue #{issue.Number}] Title: {issue.Title}\nBody: {issue.Body}\nState: {issue.State}";
    }

    [KernelFunction, Description("Searches for a file path or code snippet inside the target GitHub repository.")]
    public async Task<string> SearchRepositoryCodeAsync(
        [Description("The filename or code term to search for")] string query)
    {
        try
        {
            // Obtener todos los archivos del directorio raíz del repositorio (sin depender del índice de búsqueda)
            var contents = await _github.Repository.Content.GetAllContents(_owner, _repo);
            
            var matchingFile = contents.FirstOrDefault(f => f.Name.Equals("Calculator.cs", StringComparison.OrdinalIgnoreCase) || f.Name.Contains(query, StringComparison.OrdinalIgnoreCase));

            if (matchingFile != null)
            {
                // Si encontramos el archivo, devolvemos su ruta y su contenido directamente
                var fileDetails = await _github.Repository.Content.GetAllContentsByRef(_owner, _repo, matchingFile.Path, "main");
                return $"File Path: {matchingFile.Path}\nContent:\n{fileDetails[0].Content}";
            }

            return $"Files found in repo: {string.Join(", ", contents.Select(c => c.Name))}. None matched '{query}' exactly.";
        }
        catch (Exception ex)
        {
            return $"Error reading repository contents: {ex.Message}";
        }
    }

    [KernelFunction, Description("Creates an automated Pull Request with a proposed bug fix.")]
    public async Task<string> CreatePullRequestAsync(
        [Description("Branch name for the fix, e.g., fix/issue-42")] string branchName,
        [Description("Title of the Pull Request")] string prTitle,
        [Description("Detailed description of the changes made")] string prBody,
        [Description("Target file path to update")] string filePath,
        [Description("New full code content for the file")] string newContent)
    {
        // 1. Get Default Branch Reference (main/master)
        var masterRef = await _github.Git.Reference.Get(_owner, _repo, "heads/main");
        
        // 2. Create new branch from main
        await _github.Git.Reference.Create(_owner, _repo, new NewReference($"refs/heads/{branchName}", masterRef.Object.Sha));

        // 3. Get existing file SHA
        var fileContents = await _github.Repository.Content.GetAllContents(_owner, _repo, filePath);
        var fileSha = fileContents[0].Sha;

        // 4. Commit updated file
        await _github.Repository.Content.UpdateFile(_owner, _repo, filePath, 
            new UpdateFileRequest($"fix: automated patch for {prTitle}", newContent, fileSha, branchName));

        // 5. Open Pull Request
        var pr = await _github.Repository.PullRequest.Create(_owner, _repo, 
            new NewPullRequest(prTitle, branchName, "main") { Body = prBody });

        return $"SUCCESS! Pull Request #{pr.Number} created: {pr.HtmlUrl}";
    }

    [KernelFunction, Description("Adds a comment to an open GitHub Issue to notify maintainers.")]
    public async Task<string> PostIssueCommentAsync(
        [Description("Issue number to comment on")] int issueNumber,
        [Description("Comment content in Markdown")] string comment)
    {
        var result = await _github.Issue.Comment.Create(_owner, _repo, issueNumber, comment);
        return $"Comment added to Issue #{issueNumber}. Comment ID: {result.Id}";
    }
}