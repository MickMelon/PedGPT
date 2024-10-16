using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CitizenFX.Core;
using CitizenFX.Core.Native;
using FxMediator.Server;
using IntelliPed.FiveM.Server.Hubs;
using IntelliPed.FiveM.Server.Services;
using IntelliPed.FiveM.Shared.Requests.Heartbeats;
using IntelliPed.Messages.Heartbeats;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;

namespace IntelliPed.FiveM.Server.Controllers;

public class HeartbeatController : BaseScript
{
    private ConnectedAgentService _connectedAgentService = null!;
    private ServerMediator _mediator = null!;
    private BaseScriptProxy _baseScriptProxy = null!;

    [EventHandler("onResourceStart")]
    public void OnResourceStart(string resourceName)
    {
        if (API.GetCurrentResourceName() != resourceName)
        {
            return;
        }

        _connectedAgentService = Program.ScopedServices.GetRequiredService<ConnectedAgentService>();
        _mediator = Program.ScopedServices.GetRequiredService<ServerMediator>();
        _baseScriptProxy = Program.ScopedServices.GetRequiredService<BaseScriptProxy>();
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

        Player player = _baseScriptProxy.Players.First();

        HeartbeatRpcReply reply = await _mediator.SendToClient(player, new HeartbeatRpcRequest
        {
            PedNetworkId = agent.PedNetworkId
        });

        Heartbeat heartbeat = new()
        {
            Coordinates = new(ped.Position.X, ped.Position.Y, ped.Position.Z),
            StreetName = reply.StreetName,
            Health = reply.Health,
            NearbyPeds = reply.NearbyPeds,
            NearbyVehicles = reply.NearbyVehicles
        };

        await agentHub.Clients
            .Client(agent.ConnectionId)
            .SendAsync("Heartbeat", heartbeat);
    }
}