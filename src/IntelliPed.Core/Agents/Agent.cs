using IntelliPed.Core.Plugins;
using IntelliPed.Messages.Heartbeats;
using IntelliPed.Messages.Signals;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using IntelliPed.Core.Reasoning;
using IntelliPed.Messages.AgentStatus;
using System.Collections.Concurrent;

namespace IntelliPed.Core.Agents;

public record ThinkResult
{
    public string? Content { get; init; }
    public ThinkResultFunction? DesiredFunctionCall { get; init; }
}

public record ThinkResultFunction
{
    public required string Name { get; init; }
    public required Func<Task<string>> Call { get; init; }
}

public record ActResult
{
    public string? Content { get; init; }
}

public class Agent
{
    public Kernel Kernel { get; }
    public HubConnection HubConnection { get; }
    public PersonalInfo PersonalInfo { get; }
    public Heartbeat? LatestHeartbeat { get; private set; }

    private readonly DeepReasoning _deepReasoning;

    private readonly ConcurrentQueue<Signal> _signalQueue = [];
    private readonly CancellationTokenSource _cts = new();

    public Agent(PersonalInfo personalInfo, OpenAiOptions openAiOptions)
    {
        PersonalInfo = personalInfo;
        _deepReasoning = new();

        HubConnection = new HubConnectionBuilder()
            .WithUrl("http://localhost:5000/agent-hub")
            .Build();

        IKernelBuilder kernelBuilder = Kernel.CreateBuilder();

        kernelBuilder.Services.AddSingleton(this);

        kernelBuilder.Services.AddLogging(x => x
            .SetMinimumLevel(LogLevel.Trace)
            .AddDebug()
            .AddConsole());

        kernelBuilder.AddOpenAIChatCompletion(openAiOptions.Model, openAiOptions.ApiKey, openAiOptions.OrgId);

        kernelBuilder.Plugins
            .AddFromType<NavigationPlugin>()
            .AddFromType<SpeechPlugin>();

        Kernel = kernelBuilder.Build();
    }

    public async Task Start()
    {
        await HubConnection.StartAsync();

        HubConnection.On<DamageSignal>("DamageReceived", _signalQueue.Enqueue);
        HubConnection.On<SpeechSignal>("SpeechHeard", _signalQueue.Enqueue);
        HubConnection.On<Heartbeat>("Heartbeat", heartbeat => LatestHeartbeat = heartbeat);

        CancellationToken cancellationToken = _cts.Token;
        cancellationToken.Register(() =>
        {
            Task task = HubConnection.InvokeAsync("SetAgentStatus", new SetAgentStatusRequest { Status = EnAgentStatus.None }, CancellationToken.None);
            task.Wait(CancellationToken.None);
        });

#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
        Task.Run(() => Loop(cancellationToken), cancellationToken);
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
    }

    private async Task Loop(CancellationToken cancellationToken)
    {
        while (!_cts.IsCancellationRequested)
        {
            ThinkResult? thinkResult = await Think(cancellationToken);

            if (thinkResult is null)
            {
                continue;
            }

            ActResult? actResult = await Act(thinkResult, cancellationToken);

            await Observe(thinkResult, actResult, cancellationToken);
        }
    }

    private async Task<ThinkResult?> Think(CancellationToken cancellationToken)
    {
        // Think about what we want to do
        // Let's take a look at our signals and see if we should process them
        // We would use either deep or local reasoning

        if (!_signalQueue.TryDequeue(out Signal? signal))
        {
            return null;
        }

        await HubConnection.InvokeAsync("SetAgentStatus", new SetAgentStatusRequest { Status = EnAgentStatus.Thinking }, cancellationToken);

        // Depending on signal we may want to give additional context to the LLM.
        // E.g. for a Speech signal, the LLM would want to know the chat history.
        // Long-term history from the user that spoke to it.
        
        ThinkResult thinkResult = await _deepReasoning.Reason(this, signal.ToString(), CancellationToken.None);

        await HubConnection.InvokeAsync("SetAgentStatus", new SetAgentStatusRequest { Status = EnAgentStatus.None }, cancellationToken);

        return thinkResult;
    }

    private async Task<ActResult?> Act(ThinkResult thinkResult, CancellationToken cancellationToken)
    {
        if (thinkResult.DesiredFunctionCall is null)
        {
            Console.WriteLine("No function chosen for execution.");
            return null;
        }

        await HubConnection.InvokeAsync("SetAgentStatus", new SetAgentStatusRequest { Status = EnAgentStatus.Acting }, cancellationToken);

        Console.WriteLine($"About to execute: {thinkResult.DesiredFunctionCall.Name}");

        string content = await thinkResult.DesiredFunctionCall.Call();

        //chat.Add(new ChatMessageContent(
        //    AuthorRole.Tool,
        //    content,
        //    metadata: new Dictionary<string, object?>(1)
        //    {
        //        { OpenAIChatMessageContent.ToolIdProperty, toolCall.Id }
        //    }));

        await Task.Delay(2000, cancellationToken);

        await HubConnection.InvokeAsync("SetAgentStatus", new SetAgentStatusRequest { Status = EnAgentStatus.None }, cancellationToken);

        return new ActResult
        {
            Content = content,
        };
    }

    private async Task Observe(ThinkResult thinkResult, ActResult? actResult, CancellationToken cancellationToken)
    {
        await HubConnection.InvokeAsync("SetAgentStatus", new SetAgentStatusRequest { Status = EnAgentStatus.Observing }, cancellationToken);

        await Task.Delay(2000, cancellationToken);

        await HubConnection.InvokeAsync("SetAgentStatus", new SetAgentStatusRequest { Status = EnAgentStatus.None }, cancellationToken);
    }
}