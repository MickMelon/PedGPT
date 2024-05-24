using System;
using CitizenFX.Core;
using IntelliPed.FiveM.Server.Hubs;
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

            Debug.WriteLine($"Player {player.Handle} detected damaged ped {pedNetworkId} by {previousHealth - currentHealth}!");

            await agentHub.Clients
                .Group(pedNetworkId.ToString())
                .SendAsync("DamageReceived", new DamageSignal
                {
                    DamageAmount = previousHealth - currentHealth,
                    Source = player.Handle,
                    Weapon = "Unknown",
                });
        }
        catch (Exception exception)
        {
            Debug.WriteLine($"Error sending ped damage received event to agent: {exception.Message}");
        }
    }
}