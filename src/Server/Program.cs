
// ---------- src/Server/Program.cs ----------
// Requires packages: Microsoft.AspNetCore.OpenApi, Swashbuckle.AspNetCore (optional)

using CampusPathfinder.Agents;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = WebApplication.CreateBuilder(args);

// DI singletons (keys come from secrets or env vars)
var cfg = builder.Configuration;

builder.Services.AddSingleton<PlannerAgent>(sp =>
    new PlannerAgent(cfg["AgentEndpoint"], cfg["AgentKey"], sp.GetRequiredService<ILogger<PlannerAgent>>()));

builder.Services.AddSingleton<RagExpertAgent>(sp =>
    new RagExpertAgent(cfg["OpenAI:Endpoint"], cfg["OpenAI:Key"], cfg["OpenAI:Deployment"],
                       cfg["Search:Endpoint"],  cfg["Search:Key"]));

var app = builder.Build();

app.MapPost("/chat", async (string userMessage, PlannerAgent planner, RagExpertAgent expert) =>
{
    var plan = await planner.GetPlanAsync(userMessage);

    // naive: always call expert for now (plan evaluation skipped)
    var answer = await expert.AnswerAsync(userMessage);
    return Results.Ok(new { answer });
});

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.Run();