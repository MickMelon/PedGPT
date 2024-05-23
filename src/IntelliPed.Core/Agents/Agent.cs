using IntelliPed.Core.Plugins;
using IntelliPed.Core.Signals;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;

namespace IntelliPed.Core.Agents;

public class Agent
{
    public Kernel Kernel { get; }
    public HubConnection HubConnection { get; }
    private readonly SignalProcessor _signalProcessor;

    public Agent(OpenAiOptions openAiOptions)
    {
        _signalProcessor = new(this);

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

    public void HandleSignal(Signal signal)
    {
        _signalProcessor.Handle(signal);
    }

    public async Task Start()
    {
        await HubConnection.StartAsync();

        await HubConnection.InvokeAsync("CreatePuppet");

        _signalProcessor.Start();
    }
}