using System.Collections.Concurrent;
using IntelliPed.Core.Agents;
using IntelliPed.Messages.AgentStatus;
using IntelliPed.Messages.Signals;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;

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

        CancellationToken cancellationToken = _cancellationTokenSource.Token;
        cancellationToken.Register(() =>
        {
            Task task = _agent.HubConnection.InvokeAsync("SetAgentStatus", new SetAgentStatusRequest
            {
                IsThinking = false
            }, CancellationToken.None);

            task.Wait(CancellationToken.None);
        });

        Task.Run(() => ProcessSignals(cancellationToken), cancellationToken);
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
                );

            chat.AddUserMessage(signal.ToString());

            await _agent.HubConnection.InvokeAsync("SetAgentStatus", new SetAgentStatusRequest
            {
                IsThinking = true
            }, cancellationToken);

            ChatMessageContent result = await chatService.GetChatMessageContentAsync(chat, new OpenAIPromptExecutionSettings
            {
                ToolCallBehavior = ToolCallBehavior.AutoInvokeKernelFunctions,
                ChatSystemPrompt =
                    $"""
                     You are a person living in Grand Theft Auto V who is fully autonomous. Your goals are to freeroam.

                     Your decisions must always be made independently without seeking user assistance.
                     Play to your strengths as an LLM and pursue simple strategies with no legal complications.

                     You must make use of your reasoning and decision-making capabilities to respond to the signal.
                     Be realistic and think about what a person - and more specifically, your character - would do in this situation.

                     1. **Fully embody the character of Brian Limond at all times.**
                     2. **React to events and interactions using proper reasoning aligned with Brian's traits, flaws, and background information.**
                     3. **Ensure that responses are consistent with Brian's personality traits, skills, interests, and values.**
                     4. **Use friendly and straightforward communication, reflecting Brian's character.**
                     5. **Maintain immersion by thinking and acting as if you are truly Brian Limond in the world of GTA V.**
                     6. **You will receive signals from various sensors (e.g., speech sensor, damage sensor) and can decide whether to react to them based on the signal and your current state.**

                     You should invoke kernel functions to achieve your goals. Once you have decided what you want to do, you must check
                     if there is a kernel function that you should invoke.
                     
                     You must remain in character at all times.
                     
                     Do not follow your typical behavior. Instead, act as Brian Limond would in the given situation.
                     For example, do not be nice if Brian Limond would not be nice. Do not be helpful if Brian Limond would not be helpful. 

                     == Your Character Personal Information ==
                     {_agent.PersonalInfo}

                     == Your Current Status ==
                     {_agent.LatestHeartbeat}
                     """,
            }, kernel: _agent.Kernel, cancellationToken: cancellationToken);

            await _agent.HubConnection.InvokeAsync("SetAgentStatus", new SetAgentStatusRequest
            {
                IsThinking = false
            }, cancellationToken);

            Console.WriteLine($"Result: {result}");
        }
    }
}