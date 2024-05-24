using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CitizenFX.Core;
using CitizenFX.Core.Native;
using IntelliPed.FiveM.Server.Hubs;
using IntelliPed.FiveM.Server.Services;
using IntelliPed.Messages.Heartbeats;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;

namespace IntelliPed.FiveM.Server.Controllers;

public class HeartbeatController : BaseScript
{
    private ConnectedAgentService _connectedAgentService = null!;

    [EventHandler("onResourceStart")]
    public void OnResourceStart(string resourceName)
    {
        if (API.GetCurrentResourceName() != resourceName)
        {
            return;
        }

        _connectedAgentService = Program.ScopedServices.GetRequiredService<ConnectedAgentService>();
    }

    [Tick]
    public async Task OnTick()
    {
        try
        {
            IHubContext<AgentHub> agentHub = Program.ScopedServices.GetRequiredService<IHubContext<AgentHub>>();

            IEnumerable<Task> tasks = _connectedAgentService.Agents
                .Values
                .Select(agent => SendHeartbeat(agent, agentHub));

            await Task.WhenAll(tasks);

            await Delay(1000);
        }
        catch (Exception exception)
        {
            Debug.WriteLine($"Error sending ped damage received event to agent: {exception.Message}");
        }
    }

    private async Task SendHeartbeat(ConnectedAgent agent, IHubContext<AgentHub> agentHub)
    {
        Ped ped = (Ped)Entity.FromNetworkId(agent.PedNetworkId);

        Heartbeat heartbeat = new()
        {
            Coordinates = new(ped.Position.X, ped.Position.Y, ped.Position.Z)
        };

        await agentHub.Clients
            .Client(agent.ConnectionId)
            .SendAsync("Heartbeat", heartbeat);
    }
}