using System;
using CitizenFX.Core;
using IntelliPed.FiveM.Server.Hubs;
using IntelliPed.FiveM.Server.Services;
using IntelliPed.Messages.Signals;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;

namespace IntelliPed.FiveM.Server.Controllers;

public class DamageController : BaseScript
{
    [EventHandler("PedDamaged")]
    private async void OnPedDamaged([FromSource] Player player, int pedNetworkId, int previousHealth, int currentHealth)
    {
        try
        {
            IHubContext<AgentHub> agentHub = Program.ScopedServices.GetRequiredService<IHubContext<AgentHub>>();
            ConnectedAgentService connectedAgentService = Program.ScopedServices.GetRequiredService<ConnectedAgentService>();

            if (!connectedAgentService.TryGetByPedNetworkId(pedNetworkId, out ConnectedAgent? agent))
            {
                Debug.WriteLine($"Player {player.Handle} detected damaged ped {pedNetworkId} but no agent is connected to it.");
                return;
            }

            Debug.WriteLine($"Player {player.Handle} detected damaged ped {pedNetworkId} by {previousHealth - currentHealth}!");

            await agentHub.Clients
                .Client(agent!.ConnectionId)
                .SendAsync("DamageReceived", new DamageSignal
                {
                    DamageAmount = previousHealth - currentHealth,
                    SourcePedNetworkId = player.Character.NetworkId,
                    Weapon = "Unknown",
                });
        }
        catch (Exception exception)
        {
            Debug.WriteLine($"Error sending ped damage received event to agent: {exception.Message}");
        }
    }
}