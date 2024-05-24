using System.ComponentModel;
using IntelliPed.Core.Agents;
using IntelliPed.Messages.Speech;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.SemanticKernel;

namespace IntelliPed.Core.Plugins;

public class SpeechPlugin
{
    [KernelFunction]
    [Description("Speaks out loud in the world so that other peds can hear you.")]
    public async Task<string> Speak(
        Kernel kernel,
        [Description("What to speak.")] string message)
    {
        Agent agent = kernel.GetRequiredService<Agent>();

        await agent.HubConnection.InvokeAsync("Speak", new SpeakRequest
        {
            Message = message,
        });

        Console.WriteLine("Successfully spoke");
        return "Successfully spoke.";
    }
}