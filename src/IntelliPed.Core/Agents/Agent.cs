using IntelliPed.Core.Plugins;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Planning.Handlebars;

#pragma warning disable SKEXP0060

namespace IntelliPed.Core.Agents;

public class Agent
{
    public int PedNetworkId { get; }

    private readonly Kernel _kernel;
    
    public Agent(int pedNetworkId, OpenAiOptions openAiOptions)
    {
        PedNetworkId = pedNetworkId;

        IKernelBuilder kernelBuilder = Kernel.CreateBuilder();

        kernelBuilder.Services.AddSingleton(this);

        kernelBuilder.Services.AddLogging(_ => _
            .SetMinimumLevel(LogLevel.Trace)
            .AddDebug()
            .AddConsole());

        kernelBuilder.AddOpenAIChatCompletion(openAiOptions.Model, openAiOptions.ApiKey, openAiOptions.OrgId);
        kernelBuilder.Plugins.AddFromType<NavigationPlugin>();

        _kernel = kernelBuilder.Build();
    }

    /// <summary>
    /// The agent thinks about its current state and goals.
    /// </summary>
    public async Task Think()
    {
        // Give the LLM its current state, goals, sensor information, etc. and let it think.
        HandlebarsPlanner planner = new();
        HandlebarsPlan plan = await planner.CreatePlanAsync(_kernel, "Travel to Grove Street.");
        
        Console.WriteLine("\nThe plan:\n");
        Console.WriteLine(plan);
        Console.WriteLine("\n====================\n");

        string result = await plan.InvokeAsync(_kernel);
        Console.WriteLine("The result:\n");
        Console.WriteLine(result);
    }

    /// <summary>
    /// The agent acts on its current state and goals.
    /// </summary>
    public async Task Act()
    {

    }

    /// <summary>
    /// The agent observes its environment.
    /// </summary>
    public async Task Observe()
    {

    }
}