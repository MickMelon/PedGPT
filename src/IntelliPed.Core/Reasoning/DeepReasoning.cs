using IntelliPed.Core.Agents;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using System.Text.Json;

namespace IntelliPed.Core.Reasoning;

public class DeepReasoning
{
    public async Task<ThinkResult> Reason(
        Agent agent, 
        string prompt, 
        CancellationToken cancellationToken)
    {
        IChatCompletionService chatService = agent.Kernel.GetRequiredService<IChatCompletionService>();

        ChatHistory chat = [];

        chat.AddUserMessage(prompt);

        var result = (OpenAIChatMessageContent)await chatService.GetChatMessageContentAsync(chat, new OpenAIPromptExecutionSettings
        {
            ToolCallBehavior = ToolCallBehavior.EnableKernelFunctions,
            Temperature = 0.5f,
            ChatSystemPrompt =
                $"""
                     You are a person living in Grand Theft Auto V who is fully autonomous. Your goals are to freeroam.

                     Your decisions must always be made independently without seeking user assistance.
                     Play to your strengths as an LLM and pursue simple strategies with no legal complications.

                     You must make use of your reasoning and decision-making capabilities to respond to the signal.
                     Be realistic and think about what your character would do in this situation.

                     You should invoke kernel functions to achieve your goals.
                     
                     **Personal Information**
                     {agent.PersonalInfo}

                     **Current Status**
                     {agent.LatestHeartbeat?.ToString() ?? "All good."}
                     """,
        }, kernel: agent.Kernel, cancellationToken);

        Console.WriteLine($"Result: {result}");

        return new ThinkResult
        {
            Content = result.Content,
            DesiredFunctionCall = result.ToolCalls.Count == 0 ? null :
                new ThinkResultFunction
                {
                    Name = result.ToolCalls[0].FunctionName,
                    Call = async () => agent.Kernel.Plugins.TryGetFunctionAndArguments(result.ToolCalls[0], out KernelFunction? function, out KernelArguments? arguments)
                        ? JsonSerializer.Serialize((await function.InvokeAsync(agent.Kernel, arguments, cancellationToken)).GetValue<object>())
                        : "Function not found."
                }
        };
    }
}