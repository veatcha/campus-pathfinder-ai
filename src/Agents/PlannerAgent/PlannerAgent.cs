// ---------- src/Agents/PlannerAgent/PlannerAgent.cs ----------
// Requires package: Azure.AI.Agents (preview), Azure.AI.OpenAI, Microsoft.Extensions.Logging

using Azure.AI.Agents; // hypothetical namespace
using Azure.AI.OpenAI;
using Microsoft.Extensions.Logging;

namespace CampusPathfinder.Agents;

public class PlannerAgent
{
    private readonly AgentClient _client;
    private readonly ILogger<PlannerAgent> _log;

    public PlannerAgent(string endpoint, string apiKey, ILogger<PlannerAgent> log)
    {
        _client = new AgentClient(new Uri(endpoint), new AzureKeyCredential(apiKey));
        _log    = log;
    }

    public async Task<AgentPlan> GetPlanAsync(string userMessage)
    {
        var request = new PlanningRequest
        {
            SystemPrompt = "You are the Campus Pathfinder planner. Decide which tools the expert agent should call to answer higher-ed student queries.",
            UserMessage  = userMessage,
            // Tools registered in Agent Service UI or SDK
        };

        var response = await _client.GetPlanAsync(request);
        _log.LogInformation("Planner returned plan: {Plan}", response.Plan);
        return response.Plan;
    }
}
