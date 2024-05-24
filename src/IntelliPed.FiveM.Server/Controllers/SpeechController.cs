using CitizenFX.Core;
using IntelliPed.FiveM.Server.Hubs;
using IntelliPed.Messages.Signals;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace IntelliPed.FiveM.Server.Controllers;

public class SpeechController : BaseScript
{
    [EventHandler("chatMessage")]
    private void OnChatMessage(int source, string name, string message)
    {
        try
        {
            IHubContext<AgentHub> agentHub = Program.ScopedServices.GetRequiredService<IHubContext<AgentHub>>();

            Debug.WriteLine($"Player {source} sent chat message: {message}");

            agentHub.Clients
                .All
                .SendAsync("SpeechHeard", new SpeechSignal
                {
                    Message = message,
                });

        }
        catch (Exception exception)
        {
            Debug.WriteLine($"Error sending chat message to agent: {exception.Message}");
        }
    }
}