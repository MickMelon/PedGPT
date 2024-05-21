using IntelliPed.Core.Plugins;
using IntelliPed.Core.Signals;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;

namespace IntelliPed.Core.Agents;

public class Agent
{
    public int PedNetworkId { get; }
    public Kernel Kernel { get; }
    public SignalProcessor SignalProcessor { get; }
    
    public Agent(int pedNetworkId, OpenAiOptions openAiOptions)
    {
        PedNetworkId = pedNetworkId;
        SignalProcessor = new(this);

        HubConnection = new HubConnectionBuilder()
            .WithUrl("http://localhost:5000/agent-hub")
            .Build();

        IKernelBuilder kernelBuilder = Kernel.CreateBuilder();

        kernelBuilder.Services.AddSingleton(this);
        kernelBuilder.Services.AddLogging(_ => _
            .SetMinimumLevel(LogLevel.Trace)
            .AddDebug()
            .AddConsole());
        kernelBuilder.AddOpenAIChatCompletion(openAiOptions.Model, openAiOptions.ApiKey, openAiOptions.OrgId);
        kernelBuilder.Plugins
            .AddFromType<NavigationPlugin>()
            .AddFromType<SpeechPlugin>();

        Kernel = kernelBuilder.Build();
    }
}