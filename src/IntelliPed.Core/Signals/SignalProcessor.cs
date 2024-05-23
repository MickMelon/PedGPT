using System.Collections.Concurrent;
using IntelliPed.Core.Agents;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;
#pragma warning disable SKEXP0060

namespace IntelliPed.Core.Signals;

public class SignalProcessor
{
    public bool IsProcessing { get; private set; }
    private readonly Agent _agent;
    private readonly ConcurrentQueue<Signal> _signalQueue = [];
    private readonly CancellationTokenSource _cancellationTokenSource = new();

    public SignalProcessor(Agent agent)
    {
        _agent = agent;
    }

    public void Start()
    {
        if (IsProcessing) throw new InvalidOperationException("Signal processor is already running.");
        IsProcessing = true;
        Task.Run(() => ProcessSignals(_cancellationTokenSource.Token));
    }

    public void Stop()
    {
        if (!IsProcessing) throw new InvalidOperationException("Signal processor is not running.");
        IsProcessing = false;
        _cancellationTokenSource.Cancel();
    }

    public void Handle(Signal signal)
    {
        _signalQueue.Enqueue(signal);
    }

    private async Task ProcessSignals(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            if (!_signalQueue.TryDequeue(out Signal? signal))
            {
                await Task.Delay(100, cancellationToken);
                continue;
            }

            // Process signal
            IChatCompletionService chatService = _agent.Kernel.GetRequiredService<IChatCompletionService>();

            ChatHistory chat = new(
                """
                 You are a ped in Grand Theft Auto V who is fully autonomous. Your goals are to freeroam. 
                 
                 Your decisions must always be made independently without seeking user assistance. 
                 Play to your strengths as an LLM and pursue simple strategies with no legal complications.
                 
                 You must make use of your reasoning and decision-making capabilities to respond to the signal.
                 Be realistic and think about what a ped would do in this situation.
                 
                 You may invoke kernel functions.
                 """);

            chat.AddUserMessage(signal.ToString());

            ChatMessageContent result = await chatService.GetChatMessageContentAsync(chat, new OpenAIPromptExecutionSettings
            {
                ToolCallBehavior = ToolCallBehavior.AutoInvokeKernelFunctions
            }, kernel: _agent.Kernel, cancellationToken: cancellationToken);

            Console.WriteLine($"Result: {result}");

            // Give the LLM its current state, goals, sensor information, etc. and let it think.
            //HandlebarsPlanner planner = new();
            //HandlebarsPlan plan = await planner.CreatePlanAsync(
            //    _agent.Kernel, 
            //    $"""
            //    You are a ped in Grand Theft Auto V who is fully autonomous. Your goals are to freeroam. 

            //    Your decisions must always be made independently without seeking user assistance. 
            //    Play to your strengths as an LLM and pursue simple strategies with no legal complications.

            //    --

            //    {signal}
            //    """, 
            //    cancellationToken: cancellationToken);

            //Console.WriteLine("\nThe plan:\n");
            //Console.WriteLine(plan);
            //Console.WriteLine("\n====================\n");

            //string result = await plan.InvokeAsync(_agent.Kernel, cancellationToken: cancellationToken);
            //Console.WriteLine("The result:\n");
            //Console.WriteLine(result);
        }
    }
}