// ---------- src/Agents/RagExpertAgent/RagExpertAgent.cs ----------
// Requires packages: Microsoft.SemanticKernel, Azure.Search.Documents

using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using Azure.Search.Documents;
using Azure.Search.Documents.Models;

namespace CampusPathfinder.Agents;

public class RagExpertAgent
{
    private readonly Kernel _kernel;
    private readonly SearchClient _search;

    public RagExpertAgent(string openAiEndpoint, string openAiKey, string deploymentName,
                          string searchEndpoint, string searchKey)
    {
        _kernel = Kernel.CreateBuilder()
                         .AddAzureOpenAIChatCompletion(deploymentName, openAiEndpoint, openAiKey)
                         .Build();

        _search = new SearchClient(new Uri(searchEndpoint), "knowledge-index", new AzureKeyCredential(searchKey));
    }

    public async Task<string> AnswerAsync(string question)
    {
        // 1) Retrieve top docs
        var opts = new SearchOptions { Size = 5, QueryType = SearchQueryType.Semantic, QueryLanguage = "en-us" };
        var results = await _search.SearchAsync<SearchDocument>(question, opts);
        var passages = string.Join("\n\n", results.Value.GetResults()
                                          .Select(r => r.Document.ContainsKey("answer")
                                                       ? r.Document["answer"]
                                                       : r.Document["description"])));

        // 2) Compose prompt
        string prompt = $"""
You are a helpful university concierge. Use the following context to answer.

{passages}

Question: {question}
Answer:
""";
        // 3) Chat completion
        var chat = _kernel.GetRequiredService<IAIService>();
        var response = await chat.GetCompletionsAsync(prompt, maxTokens: 512);

        return response.First();
    }
}