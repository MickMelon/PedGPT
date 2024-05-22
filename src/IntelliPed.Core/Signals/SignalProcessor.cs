using System.Collections.Concurrent;
using IntelliPed.Core.Agents;
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

            ChatHistory chat = new("You are a ped living in the city of Los Santos, San Andreas.");

            chat.AddUserMessage($"You just have received the following signal: {signal}. How would you like to react?");

            ChatMessageContent result = await chatService.GetChatMessageContentAsync(chat, new OpenAIPromptExecutionSettings
            {
                ToolCallBehavior = ToolCallBehavior.AutoInvokeKernelFunctions
            }, cancellationToken: cancellationToken);

            Console.WriteLine($"Result: {result}");
        }
    }
}