using System;
using CitizenFX.Core;
using IntelliPed.FiveM.Messages.Sensors;
using IntelliPed.FiveM.Server.Hubs;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;

namespace IntelliPed.FiveM.Server;

public class DamageDetector : BaseScript
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
                .SendAsync("SpeechHeard", new SpeechHeardEvent
                {
                    Message = message,
                });

        }
        catch (Exception exception)
        {
            Debug.WriteLine($"Error sending chat message to agent: {exception.Message}");
        }
    }

    [EventHandler("PedDamaged")]
    private async void OnPedDamaged([FromSource] Player player, int pedNetworkId, int previousHealth, int currentHealth)
    {
        try
        {
            IHubContext<AgentHub> agentHub = Program.ScopedServices.GetRequiredService<IHubContext<AgentHub>>();

            Debug.WriteLine($"Player {player.Handle} detected damaged ped {pedNetworkId} by {previousHealth - currentHealth}!");

            await agentHub.Clients
                .Group(pedNetworkId.ToString())
                .SendAsync("DamageReceived", new DamageReceivedEvent
                {
                    PreviousHealth = previousHealth,
                    CurrentHealth = currentHealth,
                });
        }
        catch (Exception exception)
        {
            Debug.WriteLine($"Error sending ped damage received event to agent: {exception.Message}");
        }
    }
}